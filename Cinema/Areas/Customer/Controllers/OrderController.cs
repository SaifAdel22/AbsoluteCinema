using AbsoluteCinema.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{
    [Area(AreaConstants.CUSTOMER_AREA)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            var orders = await _orderService.GetOrdersForUserAsync(userId);
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int orderId)
        {
            string userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Paid)
            {
                TempData["Error"] = "Order not found or cannot be refunded.";
                return RedirectToAction(nameof(Index));
            }
            if (order.MovieDateTime <= DateTime.Now.AddHours(24))
            {
                TempData["Error"] = "Refund is not allowed less than 24 hours before the movie show time.";
                return RedirectToAction(nameof(Index));
            }
            //Saif adelll


            bool success = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);

            if (success)
            {
                TempData["Success"] = "Order refunded successfully and seats have been released!";
            }
            else
            {
                TempData["Error"] = "Failed to process refund.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

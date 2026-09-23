using AbsoluteCinema.Models;
using AbsoluteCinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMovieService _movieService; 

        public OrderController(IOrderService orderService, IMovieService movieService)
        {
            _orderService = orderService;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index(int? movieId)
        {
            var orders = await _orderService.GetAllOrdersForAdminAsync(movieId);

            ViewBag.Movies =  _movieService.GetAll();
            ViewBag.SelectedMovieId = movieId;

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!success)
            {
                TempData["Error"] = " Somthing didn't work";
            }
            else
            {
                TempData["Success"] = "Updated Succecfully.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
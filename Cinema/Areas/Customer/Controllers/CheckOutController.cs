using AbsoluteCinema.Data;
using AbsoluteCinema.Models;
using AbsoluteCinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace AbsoluteCinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            UserManager<ApplicationUser> userManager,
            ICartService cartService,
            IUnitOfWork unitOfWork,
            ApplicationDbContext applicationDbContext,
            ILogger<CheckoutController> logger)
        {
            _userManager = userManager;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            if (TempData["fromPaymentAction"] is null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

            try
            {
                var order = _unitOfWork.orderRepository.GetOne(e => e.Id == orderId && e.ApplicationUserId == user.Id);
                if (order is null || order.Status == OrderStatus.Paid) return NotFound();

                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.UpdateAt = DateTime.Now;
                order.Status = OrderStatus.Paid;

                await _unitOfWork.orderRepository.CommitAsync();

                var cart = _cartService.GetByUserID(user.Id);
                if (cart != null && cart.Seats != null)
                {
                    foreach (var seat in cart.Seats)
                    {
                        seat.IsBooked = true;
                        seat.CartId = null;
                        _unitOfWork.seatRepository.Update(seat);
                    }
                }

                await _cartService.ClearUserCartAsync(user.Id);
                await _unitOfWork.seatRepository.CommitAsync();

                await transaction.CommitAsync();
                TempData["Success"] = "Payment processed and tickets confirmed successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
            }

            return View();
        }

        public IActionResult Cancel()
        {
            if (TempData["fromPaymentAction"] is null) return NotFound();

            return View();
        }
    }
}
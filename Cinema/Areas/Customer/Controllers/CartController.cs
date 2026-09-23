using AbsoluteCinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace AbsoluteCinema.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? promo = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var cartItems = _cartService.GetAll(userId);

            if (!string.IsNullOrEmpty(promo))
            {
                var promotion = _unitOfWork.promotionRepository.GetOne(p => p.Code == promo && p.Status == true && p.ValidTo >= DateTime.Now && p.MaxUsage >= 1);

                bool isValidPromo = false;

                if (promotion != null)
                {
                    foreach (var item in cartItems)
                    {
                        if (promotion.MovieId == null || promotion.MovieId == item.MovieId)
                        {
                            var discountAmount = item.Movie.Price * (promotion.Discount / 100m);
                            item.CurrentPrice = item.Movie.Price - discountAmount;

                            isValidPromo = true;
                            break;
                        }
                    }
                }

                if (isValidPromo)
                    TempData["Success"] = "Apply promotion successfully";
                else
                    TempData["Error"] = "Invalid promo code, invalid movie or expired";
            }

            return View(cartItems);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int movieId, List<int> seatIds)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (seatIds == null || !seatIds.Any())
            {
                TempData["Error"] = "Please select at least one seat!";
                return RedirectToAction("Details", "Home", new { id = movieId });
            }

            try
            {
                await _cartService.Add(movieId, userId, seatIds);
                TempData["Success"] = "Seats booked and added to cart successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Home", new { id = movieId });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Increment(int id)
        {
            var cart = _cartService.GetByID(id);
            if (cart == null) return NotFound();

            TempData["Info"] = "اختار كرسي إضافي من الشاشة عشان تزود الحجز.";
            return RedirectToAction("Details", "Home", new { area = "Customer", id = cart.MovieId });
        }
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Decrement(int id)
            {
                var userId = _userManager.GetUserId(User);
                await _cartService.DecrementQuantity(id, userId);

                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                var userId = _userManager.GetUserId(User);
                await _cartService.Delete(id, userId);

                return RedirectToAction(nameof(Index));
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(CancellationToken ct = default)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cart = _cartService.GetByUserID(user.Id);
            if (cart == null || cart.Seats == null || !cart.Seats.Any())
            {
                TempData["Error"] = "Your cart is empty, cannot proceed with payment!";
                return RedirectToAction(nameof(Index));
            }

            var existingOrder = _unitOfWork.orderRepository.GetOne(o => o.ApplicationUserId == user.Id && o.MovieId == cart.MovieId && o.Status == OrderStatus.Pending);

            if (existingOrder != null)
            {
                existingOrder.TotalPrice = cart.CurrentPrice;
                _unitOfWork.orderRepository.Update(existingOrder);
                await _unitOfWork.orderRepository.CommitAsync(ct);
            }
            else
            {
                existingOrder = new Order
                {
                    ApplicationUserId = user.Id,
                    MovieId = cart.MovieId,
                    Status = OrderStatus.Pending,
                    TotalPrice = cart.CurrentPrice,
                    CreateAt = DateTime.Now
                };
                await _unitOfWork.orderRepository.CreateAsync(existingOrder, ct);
                await _unitOfWork.orderRepository.CommitAsync(ct);
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = cart.Movie?.Name ?? "Movie Tickets",
                        Description = $"Booking seats for movie: {cart.Movie?.Name}"
                    },
                    UnitAmount = (long)(cart.CurrentPrice * 100),
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={existingOrder.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            existingOrder.SessionId = session.Id;
            _unitOfWork.orderRepository.Update(existingOrder);
            await _unitOfWork.orderRepository.CommitAsync(ct);

            TempData["fromPaymentAction"] = Guid.NewGuid();

            return Redirect(session.Url);
        }
    }
    
}
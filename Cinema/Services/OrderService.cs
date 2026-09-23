using AbsoluteCinema.Data;
using AbsoluteCinema.Models;
using Microsoft.EntityFrameworkCore;

namespace AbsoluteCinema.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IUnitOfWork _unitOfwork; 
        private readonly ICartService _cartService; 


        public OrderService(ApplicationDbContext context, IUnitOfWork unitOfwork , ICartService cartService)
        {
            _context = context;
            _unitOfwork = unitOfwork;
            _cartService = cartService;
        }

        public async Task<IEnumerable<AdminOrderVM>> GetAllOrdersForAdminAsync(int? movieId = null)
        {
            var query = _unitOfwork.orderRepository.Get().Include(o => o.Movie).Include(o => o.Seats).Include(o=>o.ApplicationUser).AsQueryable();


            if (movieId.HasValue && movieId.Value > 0)
            {
                query = query.Where(o => o.MovieId == movieId.Value);
            }
            return await query.Select(o => new AdminOrderVM
            {

                Id = o.Id,
                MovieTitle = o.Movie.Name,
                MoviePosterUrl = o.Movie.MainImg,
                UserName = o.ApplicationUser.UserName,
                UserEmail = o.ApplicationUser.Email,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                SeatNumbers = o.Seats.Select(s => s.SeatNumber.ToString()).ToList(),
                CreatedAt = o.CreateAt
            }).ToListAsync(); 




        }

        public async Task<AdminOrderVM> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                            .Include(o => o.Movie)
                            .Include(o => o.ApplicationUser)
                            .Include(o => o.Seats)
                            .Where(o => o.Id == orderId)
                            .Select(o => new AdminOrderVM
                            {
                                Id = o.Id,
                                MovieTitle = o.Movie.Name,
                                MoviePosterUrl = o.Movie.MainImg,
                                UserName = o.ApplicationUser.UserName,
                                UserEmail = o.ApplicationUser.Email,
                                TotalPrice = o.TotalPrice,
                                Status = o.Status,
                                SeatNumbers = o.Seats.Select(s => s.SeatNumber.ToString()).ToList(),
                                CreatedAt = o.CreateAt
                            })
                            .FirstOrDefaultAsync();

            return order;
        }

        public async Task<IEnumerable<AdminOrderVM>> GetOrdersForUserAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Movie)
                .Include(o => o.ApplicationUser)
                .Include(o => o.Seats)
                .Where(o => o.ApplicationUserId == userId)
                .OrderByDescending(o => o.CreateAt)
                .Select(o => new AdminOrderVM
                {
                    Id = o.Id,
                    MovieTitle = o.Movie.Name,
                    MoviePosterUrl = o.Movie.MainImg,
                    UserName = o.ApplicationUser.UserName,
                    UserEmail = o.ApplicationUser.Email,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    SeatNumbers = o.Seats.Select(s => s.SeatNumber.ToString()).ToList(),
                    CreatedAt = o.CreateAt
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Seats)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            order.Status = newStatus;
            order.UpdateAt = System.DateTime.Now;

            if (newStatus == OrderStatus.Cancelled || newStatus == OrderStatus.Refunded)
            {
                var seatIds = order.Seats.Select(s => s.Id).ToList();

                var seatsToRelease = await _context.Seats
                    .Where(s => s.MovieId == order.MovieId && (s.ApplicationUserId == order.ApplicationUserId || seatIds.Contains(s.Id)))
                    .ToListAsync();

                foreach (var seat in seatsToRelease)
                {
                    seat.IsBooked = false;
                    seat.CartId = null;
                    seat.ApplicationUserId = null;
                    _context.Seats.Update(seat);
                }

                var userCart = await _context.Carts
                    .Include(c => c.Seats)
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == order.ApplicationUserId && c.MovieId == order.MovieId);

                if (userCart != null)
                {
                    foreach (var seat in seatsToRelease)
                    {
                        seat.CartId = null;
                    }

                    _context.Carts.Remove(userCart);
                }
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
using AbsoluteCinema.Data;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AbsoluteCinema.Services
{
    public class CartService : ICartService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Add(int movieId, string userId, List<int> seatIds)
        {
            var movie = _unitOfWork.movieRepository.GetOne(m => m.Id == movieId);
            if (movie.Status == false)
            {
                throw new InvalidOperationException("Cannot add an inactive movie to the cart.");
            }
            if (seatIds == null || !seatIds.Any())
            {
                throw new InvalidOperationException("Please select at least one seat.");
            }

            var cart = _unitOfWork.cartRepository.GetOne(
                c => c.MovieId == movieId && c.ApplicationUserId == userId
            );

            if (cart == null)
            {
                cart = new Cart
                {
                    MovieId = movieId,
                    ApplicationUserId = userId,
                    Count = 0,
                    CurrentPrice = 0
                };
                await _unitOfWork.cartRepository.CreateAsync(cart);
                await _unitOfWork.cartRepository.CommitAsync();

                cart = _unitOfWork.cartRepository.GetOne(c => c.Id == cart.Id);
            }

            foreach (var seatId in seatIds)
            {
                var seat = _unitOfWork.seatRepository.GetOne(s => s.Id == seatId && s.MovieId == movieId);
                if (seat != null && !seat.IsBooked)
                {
                    seat.IsBooked = true;
                    seat.CartId = cart.Id;
                    seat.ApplicationUserId = userId;
                    _unitOfWork.seatRepository.Update(seat);
                }
            }

            await _unitOfWork.cartRepository.CommitAsync();

            var allCartSeats = _unitOfWork.seatRepository.Get(s => s.CartId == cart.Id && s.MovieId == movieId).ToList();

            cart.Count = allCartSeats.Count;
            cart.CurrentPrice = movie.Price * cart.Count; 

            _unitOfWork.cartRepository.Update(cart);
            await _unitOfWork.cartRepository.CommitAsync();

            var existingOrder = _unitOfWork.orderRepository.GetOne(o => o.ApplicationUserId == userId && o.MovieId == movieId && o.Status == OrderStatus.Pending);

            if (existingOrder != null)
            {
                existingOrder.TotalPrice = cart.CurrentPrice;
                _unitOfWork.orderRepository.Update(existingOrder);
            }
            else
            {
                var order = new Order
                {
                    ApplicationUserId = userId,
                    MovieId = movieId,
                    TotalPrice = cart.CurrentPrice,
                    Status = OrderStatus.Pending,
                    CreateAt = DateTime.Now
                };
                await _unitOfWork.orderRepository.CreateAsync(order);
            }

            await _unitOfWork.orderRepository.CommitAsync();
        }


        public async Task DecrementQuantity(int cartId, string userId)
        {
            var cartItem = _unitOfWork.cartRepository.GetOne(c => c.Id == cartId && c.ApplicationUserId == userId, includes: new Expression<Func<Cart, object>>[] { c => c.Seats, c => c.Movie });

            if (cartItem == null)
                return;

            var releseseat = cartItem.Seats.FirstOrDefault(c => c.CartId == cartId && c.ApplicationUserId == userId);


            if (releseseat != null)
            {
                releseseat.IsBooked = false;
                releseseat.CartId = null;
                releseseat.ApplicationUserId = null;
                _unitOfWork.seatRepository.Update(releseseat);
            }
            if (cartItem.Count > 1)
            {
                cartItem.Count--;
                cartItem.CurrentPrice = cartItem.Movie.Price * cartItem.Count;
                _unitOfWork.cartRepository.Update(cartItem);
            }
            else
            {
                _unitOfWork.cartRepository.Delete(cartItem);
            }

            await _unitOfWork.cartRepository.CommitAsync();


        }

        /*public async Task IncrementQuantity(int cartId, string userId)
        {
            var cartItem = _unitOfWork.cartRepository.GetOne(c => c.Id == cartId && c.ApplicationUserId == userId, includes: new Expression<Func<Cart, object>>[] { c => c.Seats, c => c.Movie });

            if (cartItem == null)
                return;

            var releseseat = cartItem.Seats.FirstOrDefault(c => c.CartId == cartId && c.ApplicationUserId == userId);


            if (releseseat != null)
            {
                releseseat.IsBooked = false;
                releseseat.CartId = null;
                releseseat.ApplicationUserId = null;
                _unitOfWork.seatRepository.Update(releseseat);
            }
            if (cartItem.Count > 1)
            {
                cartItem.Count--;
                cartItem.CurrentPrice = cartItem.Movie.Price * cartItem.Count;
                _unitOfWork.cartRepository.Update(cartItem);
            }
            else
            {
                _unitOfWork.cartRepository.Delete(cartItem);
            }

            await _unitOfWork.cartRepository.CommitAsync();
        }*/


        public async Task Delete(int cartId, string userId)
        {
            var cart = _unitOfWork.cartRepository.GetOne(
                 c => c.Id == cartId && c.ApplicationUserId == userId,
                 includes: new Expression<Func<Cart, object>>[] { c => c.Seats }
             );

            if (cart == null) return;

            if (cart.Seats != null)
            {
                foreach (var seat in cart.Seats)
                {
                    seat.IsBooked = false;
                    seat.CartId = null;
                    seat.ApplicationUserId = null;
                    _unitOfWork.seatRepository.Update(seat);
                }
            }

            var pendingOrder = _unitOfWork.orderRepository.GetOne(
                o => o.ApplicationUserId == userId && o.MovieId == cart.MovieId && o.Status == OrderStatus.Pending
            );

            if (pendingOrder != null)
            {
                _unitOfWork.orderRepository.Delete(pendingOrder);
            }

            _unitOfWork.cartRepository.Delete(cart);
            await _unitOfWork.cartRepository.CommitAsync();
        }

        public bool Exist(int movieId, string userId)
        {
            var item = _unitOfWork.cartRepository.GetOne(c => c.MovieId == movieId && c.ApplicationUserId == userId);
            return item != null;
        }

        public IEnumerable<Cart> GetAll(string userId)
        {
            return _unitOfWork.cartRepository
                .Get(c => c.ApplicationUserId == userId, includes: new Expression<Func<Cart, object>>[] { c => c.Movie })
                .ToList();
        }

        public Cart GetByID(int id)
        {
            return _unitOfWork.cartRepository.GetOne(e => e.Id == id, includes: new Expression<Func<Cart, object>>[] { e => e.Movie });
        }

        public Cart GetByUserID(string id)
        {
            return _unitOfWork.cartRepository.GetOne(
                i => i.ApplicationUserId == id,
                includes: new Expression<Func<Cart, object>>[] { c => c.Seats, c => c.Movie }
            );
        }
        public async Task ClearUserCartAsync(string userId)
        {
            var carts = _unitOfWork.cartRepository.Get(c => c.ApplicationUserId == userId, includes: new Expression<Func<Cart, object>>[] { c => c.Seats }).ToList();

            if (carts == null || !carts.Any()) return;

            foreach (var cart in carts)
            {
                if (cart.Seats != null)
                {
                    foreach (var seat in cart.Seats)
                    {
                       
                        seat.CartId = null;
                        _unitOfWork.seatRepository.Update(seat);
                    }
                }
                _unitOfWork.cartRepository.Delete(cart);
            }

            await _unitOfWork.cartRepository.CommitAsync();
        }
    }
}
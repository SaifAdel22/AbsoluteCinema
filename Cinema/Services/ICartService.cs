using AbsoluteCinema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbsoluteCinema.Services
{
    public interface ICartService
    {
        Task Add(int movieId, string userId , List<int> seatIds);
       // Task IncrementQuantity(int cartId, string userId);
        Task DecrementQuantity(int cartId, string userId);
        Task Delete(int cartId, string userId);
        bool Exist(int movieId, string userId);
        IEnumerable<Cart> GetAll(string userId);
        Cart GetByID(int id);
        Task ClearUserCartAsync(string userId);
        Cart GetByUserID(string id);
    }
}
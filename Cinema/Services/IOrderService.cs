public interface IOrderService
{
    Task<IEnumerable<AdminOrderVM>> GetAllOrdersForAdminAsync(int? movieId = null);
    Task<AdminOrderVM> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<AdminOrderVM>> GetOrdersForUserAsync(string userId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
}
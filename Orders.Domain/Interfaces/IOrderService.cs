using Orders.Domain.Entities;
using Orders.Domain.Models;
using Orders.Domain.Models.Filters;

namespace Orders.Domain.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<Order>> GetAllOrdersAsync(OrderFilter filter);
        Task<Order> GetByIdAsync(Guid idOrder);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(Guid orderId, bool logical = false);
        Task<Guid> CreateAsync(Order order);
    }
}

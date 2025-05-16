using Orders.Domain.Entities;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces;
using Orders.Domain.Models;
using Orders.Domain.Models.Filters;

namespace Orders.Domain.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Guid> CreateAsync(Order order)
        {
            if (order.Value <= 0)
            {
                throw new BusinessException("O valor do pedido deve ser maior que zero.");
            }

            if(order.Id == Guid.Empty)
            {
                order.Id = Guid.NewGuid();
            }

            return await _orderRepository.AddAsync(order); ;
        }

        public async Task<bool> DeleteAsync(Guid orderId, bool logical = true)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if(order is null)
            {
                throw new BusinessException("Pedido não encontrado");
            }

            if(order.Status == Status.Pago)
            {
                throw new BusinessException("Não é possível deletar pedidos com status Pago (3).");
            }

            if (logical)
            {
                order.Active = false;
                _orderRepository.Update(order);
            }
            else
            {
                _orderRepository.Remove(order);
            }

            await _orderRepository.SaveChangesAsync();

            return true;
        }

        public async Task<PagedResult<Order>> GetAllOrdersAsync(OrderFilter filter)
        {
            var (items, totalCount) = await _orderRepository.GetPagedAsync(filter.PageNumber, filter.PageSize);
            
            return new PagedResult<Order>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<Order> GetByIdAsync(Guid idOrder)
        {
            var order =  await _orderRepository.GetByIdAsync(idOrder);

            if(order is null)
            {
                throw new NotFoundException($"Pedido não encontrado");
            }

            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            var exists = _orderRepository.GetByIdAsync(order.Id).Result is not null;

            if (!exists)
            {
                throw new BusinessException("Pedido não encontrado");
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return order;
        }
    }
}

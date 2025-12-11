using OrderService.Application.Models;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task CreateOrder(OrderDTO OrderDTO)
        {
            var order = new Order
            {
                CustomerId = OrderDTO.CustomerId,
                Amount = OrderDTO.Amount
            };

            await _orderRepository.CreateOrder(order);
        }

         

    }
}

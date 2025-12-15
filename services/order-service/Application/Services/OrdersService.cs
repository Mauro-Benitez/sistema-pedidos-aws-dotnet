using OrderService.Application.IMessaging;
using OrderService.Application.Models;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICreateOrderEventPublisher _createOrderEventPublisher;

        public OrdersService(IOrderRepository orderRepository, ICreateOrderEventPublisher createOrderEventPublisher)
        {
            _orderRepository = orderRepository;
            _createOrderEventPublisher = createOrderEventPublisher;
        }

        public async Task CreateOrder(OrderDTO OrderDTO)
        {
            var order = new Order
            {
                CustomerId = OrderDTO.CustomerId,
                Amount = OrderDTO.Amount
            };

            await _orderRepository.CreateOrder(order);
            await _createOrderEventPublisher.PublishAsync(order);
        }
        

    }
}

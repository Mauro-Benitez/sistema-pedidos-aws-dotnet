using OrderService.Application.Models;
using OrderService.Domain.Entities;

namespace OrderService.Application.IMessaging
{
    public interface ICreateOrderEventPublisher
    {
        Task PublishAsync(Order order);
    }
}

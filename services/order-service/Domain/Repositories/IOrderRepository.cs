using OrderService.Domain.Entities;

namespace OrderService.Domain.Repositories
{
    public interface IOrderRepository
    {

        Task CreateOrder(Order Order);
    }
}

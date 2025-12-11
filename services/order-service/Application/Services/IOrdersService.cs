using OrderService.Application.Models;
using OrderService.Domain.Entities;

namespace OrderService.Application.Services
{
    public interface IOrdersService
    {
        Task CreateOrder(OrderDTO OrderDTO);
    }
}

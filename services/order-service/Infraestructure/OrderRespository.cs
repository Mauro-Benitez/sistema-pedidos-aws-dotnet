using Amazon.DynamoDBv2.DataModel;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Infraestructure
{
    public class OrderRespository : IOrderRepository
    {
        private readonly IDynamoDBContext _context;

        public OrderRespository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task CreateOrder(Order Order)
        {
            await _context.SaveAsync(Order);
        }
    }
}

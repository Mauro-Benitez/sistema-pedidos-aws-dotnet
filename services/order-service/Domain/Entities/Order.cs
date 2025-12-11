using Amazon.DynamoDBv2.DataModel;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; } = Guid.NewGuid(); 

        [DynamoDBProperty]
        public Guid CustomerId { get; set; }

        [DynamoDBProperty]
        public decimal Amount { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; } = "CREATED";

        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DynamoDBProperty]
        public DateTime? UpdatedAt { get; set; }
    }
}

namespace OrderService.Application.Models
{
    public class OrderDTO
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}

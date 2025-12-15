using Amazon.SQS;
using Amazon.SQS.Model;
using OrderService.Application.IMessaging;
using OrderService.Domain.Entities;
using System.Text.Json;

namespace OrderService.Infraestructure.Messaging
{
    public class CreateOrderEventPublisher : ICreateOrderEventPublisher
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;
       

        public CreateOrderEventPublisher(IAmazonSQS sqsClient, IConfiguration configuration)
        {
            _sqsClient = sqsClient;       
            _queueUrl = configuration["AWS:SQSQueueUrl"];
        }

        public async Task PublishAsync(Order order)
        {
            

            var messageBody = JsonSerializer.Serialize(order);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(sendMessageRequest);
        }
    }
}

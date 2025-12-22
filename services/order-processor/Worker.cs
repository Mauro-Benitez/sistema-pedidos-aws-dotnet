using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace OrderProcessor;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;

    public Worker(ILogger<Worker> logger, IAmazonSQS sqsClient, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _queueUrl = configuration["AWS:SQSQueueUrl"];
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado. Aguardando mensagem...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 1,
                    WaitTimeSeconds = 20,
                    VisibilityTimeout = 30
                };

                var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);

                if (response.Messages != null)
                {
                    foreach (var message in response.Messages)
                    {
                        await ProcessMessageAsync(message);

                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhuma mensagem foi encontrada no momento");
                }
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila.");
                await Task.Delay(1000, stoppingToken);
            }
            ;

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessMessageAsync(Message message)
    {
        _logger.LogInformation($"Processando mensagem:{message.Body}");

        var OrderDto = JsonSerializer.Deserialize<Order>(message.Body);
        
        if(OrderDto != null)
        {
            //Simulando o processamento do pedido
            await Task.Delay(2000);

            using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();


            var order = await dbContext.LoadAsync<Order>(OrderDto.Id);

            if(order != null)
            {
                order.Status = "COMPLETED";
                order.UpdateAT = DateTime.UtcNow;

                await dbContext.SaveAsync(order);
                _logger.LogInformation($"Pedido {order.Id} atualizado para COMPLETED");

            }
        }
    }


}

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using OrderService.Application.Services;
using OrderService.Domain.Repositories;
using OrderService.Infraestructure;
using Amazon.SQS;
using Amazon.Extensions.NETCore.Setup;
using OrderService.Application.IMessaging;
using OrderService.Infraestructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços da API
builder.Services.AddControllers();

// Configura Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IOrderRepository, OrderRespository>();
builder.Services.AddScoped<ICreateOrderEventPublisher, CreateOrderEventPublisher>();
builder.Services.AddAWSService<IAmazonSQS>();

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var config = builder.Configuration.GetSection("AWS");

    return new AmazonDynamoDBClient(
        config["AccessKey"],
        config["SecretKey"],
        Amazon.RegionEndpoint.GetBySystemName(config["Region"])
    );
});

builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();

// Middleware do Swagger (somente em Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

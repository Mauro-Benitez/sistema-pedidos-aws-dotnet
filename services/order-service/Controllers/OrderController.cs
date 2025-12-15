using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.IMessaging;
using OrderService.Application.Models;
using OrderService.Application.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrdersService _orderService;  

        public OrderController(IOrdersService orderService)
        {
            _orderService = orderService;
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO order)
        {
            if (order == null) return BadRequest();

            try
            {
                await _orderService.CreateOrder(order);            
                return StatusCode(201);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        [HttpGet("test-dynamo")]
        public async Task<IActionResult> TestDynamo([FromServices] IAmazonDynamoDB client)
        {
            var tables = await client.ListTablesAsync();
            return Ok(tables.TableNames);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary;
using Otus.Microservice.TransportLibrary.Services;

namespace Otus.Microservice.Order.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController: ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IMessagePublisher<ProcessPaymentEvent> _processPaymentPublisher;

    public OrderController(
        ILogger<OrderController> logger,
        AppDbContext dbContext,
        IMessagePublisher<ProcessPaymentEvent> processPaymentPublisher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _processPaymentPublisher = processPaymentPublisher;
    }

    [HttpGet("{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrder([FromRoute] string requestId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(
            x => x.RequestId == requestId);
        if (order == null)
        {
            _logger.LogWarning("Order with request id {RequestId} is not found", requestId);
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrder(
        [FromHeader(Name = HttpHeaderKeys.RequestId)][Required] string requestId,
        [FromBody] Models.Order createdOrder)
    {
        try
        {
            if (await _dbContext.Orders.AnyAsync(x => x.RequestId == requestId))
            {
                _logger.LogWarning(
                    "Order has already created for request with id {RequestId}",
                    requestId);
                return Ok();
            }

            createdOrder.RequestId = requestId;
            await _dbContext.AddAsync(createdOrder);
            await _dbContext.SaveChangesAsync();
            _processPaymentPublisher.Send(new ProcessPaymentEvent
            {
                TransactionId = requestId,
                ProductId = createdOrder.ProductId,
                Count = createdOrder.Count,
                PaymentValue = createdOrder.Cost,
                Address = createdOrder.Address
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "create order error");
            throw;
        }
    }
}
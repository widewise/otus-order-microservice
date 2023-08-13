using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Otus.Microservice.Order.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController: ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly AppDbContext _dbContext;

    public OrderController(
        ILogger<OrderController> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
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
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "create order error");
            throw;
        }
    }
}
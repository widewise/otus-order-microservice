﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Otus.Microservice.Notification.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly AppDbContext _dbContext;

    public NotificationController(
        ILogger<NotificationController> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotification()
    {
        return Ok(await _dbContext.Notifications.ToArrayAsync());
    }
    
    [HttpGet("{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotification([FromRoute][Required] string transactionId)
    {
        var notification = await _dbContext.Notifications.FirstOrDefaultAsync(
            x => x.TransactionId == transactionId);
        if (notification == null)
        {
            _logger.LogWarning("Notification with transaction id {RequestId} is not found", transactionId);
            return NotFound();
        }

        return Ok(notification);
    }
}
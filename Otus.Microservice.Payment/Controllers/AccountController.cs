using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Payment.Models;
using Otus.Microservice.TransportLibrary;

namespace Otus.Microservice.Payment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _dbContext;

    public AccountController(
        ILogger<AccountController> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    [HttpGet("{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccount([FromRoute] string requestId)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(
            x => x.RequestId == requestId);
        if (account == null)
        {
            _logger.LogWarning("Account with request id {RequestId} is not found", requestId);
            return NotFound();
        }

        return Ok(account);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAccount(
        [FromHeader(Name = HttpHeaderKeys.RequestId)][Required] string requestId,
        [FromBody] CreateAccount createdAccount)
    {
        try
        {
            if (await _dbContext.Accounts.AnyAsync(x => x.RequestId == requestId))
            {
                _logger.LogWarning(
                    "Account has already created for request with id {RequestId}",
                    requestId);
                return Ok();
            }

            createdAccount.RequestId = requestId;
            await _dbContext.AddAsync(new Account
            {
                RequestId = requestId,
                Name = createdAccount.Name,
                NotificationAddress = createdAccount.NotificationAddress,
                Balance = 0
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "create account error");
            throw;
        }
    }

    [HttpPatch("{accountId}/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Deposit(
        [FromHeader(Name = HttpHeaderKeys.RequestId)] [Required] string requestId,
        [FromRoute] long accountId,
        [FromBody] CreateDeposit createDeposit)
    {
        try
        {
            if (await _dbContext.Transactions.AnyAsync(x => x.RequestId == requestId))
            {
                _logger.LogWarning(
                    "Transaction has already created for request with id {RequestId}",
                    requestId);
                return Ok();
            }

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                _logger.LogWarning("Account with id {AccountId} is not found", accountId);
                return NotFound();
            }

            createDeposit.RequestId = requestId;
            account.Balance += createDeposit.Value;
            await _dbContext.Transactions.AddAsync(new Transaction
            {
                RequestId = requestId,
                Type = TransactionType.Deposit,
                Value = createDeposit.Value,
            });
            _dbContext.Accounts.Update(account); 
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "deposit account error");
            throw;
        }
    }

    [HttpPatch("{accountId}/withdrawal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Withdrawal(
        [FromHeader(Name = HttpHeaderKeys.RequestId)] [Required]
        string requestId,
        [FromRoute] long accountId,
        [FromBody] CreateWithdrawal createWithdrawal)
    {
        try
        {
            if (await _dbContext.Transactions.AnyAsync(x => x.RequestId == requestId))
            {
                _logger.LogWarning(
                    "Transaction has already created for request with id {RequestId}",
                    requestId);
                return Ok();
            }

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                _logger.LogWarning("Account with id {AccountId} is not found", accountId);
                return NotFound();
            }

            if (account.Balance < createWithdrawal.Value)
            {
                _logger.LogWarning("Account has not enough balance");
                return BadRequest();
            }

            createWithdrawal.RequestId = requestId;
            account.Balance -= createWithdrawal.Value;
            await _dbContext.Transactions.AddAsync(new Transaction
            {
                RequestId = requestId,
                Type = TransactionType.Withdrawal,
                Value = createWithdrawal.Value,
            });
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "withdrawal account error");
            throw;
        }
    }
}
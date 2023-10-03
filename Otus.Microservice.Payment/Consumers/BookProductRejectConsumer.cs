using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events.Models;
using Otus.Microservice.Payment.Models;
using Otus.Microservice.TransportLibrary.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Payment.Consumers;

public class BookProductRejectConsumer : MessageConsumer<BookProductRejectEvent, IEvent>,
    IMessageConsumer<BookProductRejectEvent>
{
    private readonly ILogger<MessageConsumer<BookProductRejectEvent, IEvent>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessagePublisher<ProcessPaymentRejectEvent> _processPaymentRejectPublisher;

    public BookProductRejectConsumer(
        ILogger<MessageConsumer<BookProductRejectEvent, IEvent>> logger,
        IModel channel,
        IServiceScopeFactory scopeFactory,
        IMessagePublisher<ProcessPaymentRejectEvent> processPaymentRejectPublisher)
        : base(logger, channel, null)

    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _processPaymentRejectPublisher = processPaymentRejectPublisher;
    }

    public override async Task ExecuteEventAsync(BookProductRejectEvent message)
    {
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var transaction = await dbcontext.Transactions.FirstOrDefaultAsync(
                x => x.RequestId == message.TransactionId);
            if (transaction != null && transaction.Type == TransactionType.Withdrawal)
            {
                var account = await dbcontext.Accounts.FirstOrDefaultAsync(
                    x => x.Id == transaction.AccountId);
                if (account != null)
                {
                    account.Balance += transaction.Value;
                    dbcontext.Accounts.Update(account);
                }
                else
                {
                    _logger.LogWarning("Account with id {AccountId} is not found", transaction.AccountId);
                }

                dbcontext.Transactions.Remove(transaction);
                await dbcontext.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Withdrawal for request id {RequestId} is not found", message.TransactionId);
            }
        }

        _processPaymentRejectPublisher.Send(new ProcessPaymentRejectEvent
        {
            TransactionId = message.TransactionId,
            ProductId = message.ProductId,
        });

        _logger.LogInformation(
            "Booking product with id {ProductId} was rejected",
            message.ProductId);
    }
}
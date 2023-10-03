using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events.Models;
using Otus.Microservice.Payment.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Payment.Consumers;

public class ProcessPaymentConsumer : MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>,
    IMessageConsumer<ProcessPaymentEvent>
{
    private readonly ILogger<MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessagePublisher<NotificationSendEvent> _notificationSendPublisher;
    private readonly IMessagePublisher<BookProductEvent> _bookProductPublisher;

    public ProcessPaymentConsumer(
        ILogger<MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>> logger,
        IModel channel,
        IServiceScopeFactory scopeFactory,
        IMessagePublisher<ProcessPaymentRejectEvent>? revertMessagePublisher,
        IMessagePublisher<NotificationSendEvent> notificationSendPublisher,
        IMessagePublisher<BookProductEvent> bookProductPublisher)
        : base(logger, channel, revertMessagePublisher)

    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _notificationSendPublisher = notificationSendPublisher;
        _bookProductPublisher = bookProductPublisher;
    }

    public override async Task ExecuteEventAsync(ProcessPaymentEvent message)
    {
        if (message.PaymentValue <= 0)
        {
            _logger.LogError("Invalid payment value: {PaymentValue}", message.PaymentValue);
            await RejectEventAsync(message);
            return;
        }
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var transaction = await dbContext.Transactions.FirstOrDefaultAsync(
                x => x.RequestId == message.TransactionId);
            if (transaction == null)
            {
                var account = await dbContext.Accounts.FirstOrDefaultAsync(
                    x => x.Id == message.AccountId);
                if (account == null)
                {
                    _logger.LogError("Account with id {PaymentValue} is not found", message.AccountId);
                    await RejectEventAsync(message);
                    return;
                }

                if (account.Balance < message.PaymentValue)
                {
                    _logger.LogWarning("Account has not enough balance, notify user about it");
                    _notificationSendPublisher.Send(new NotificationSendEvent
                    {
                        TransactionId = message.TransactionId,
                        Type = NotificationType.Failed,
                        ToAddress = account.NotificationAddress,
                        Title = "Payment failed",
                        Body = "You don't have enough money on your account"
                    });
                    await RejectEventAsync(message);
                    return;
                }
                account.Balance -= message.PaymentValue;
                await dbContext.Transactions.AddAsync(new Transaction
                {
                    RequestId = message.TransactionId,
                    Type = TransactionType.Withdrawal,
                    Value = message.PaymentValue,
                });
                dbContext.Accounts.Update(account);
                await dbContext.SaveChangesAsync();

                _notificationSendPublisher.Send(new NotificationSendEvent
                {
                    TransactionId = message.TransactionId,
                    Type = NotificationType.Successed,
                    ToAddress = account.NotificationAddress,
                    Title = "Payment succeed",
                    Body = $"From your account was withdrawal {message.PaymentValue}"
                });
            }
        }

        _bookProductPublisher.Send(new BookProductEvent
        {
            TransactionId = message.TransactionId,
            ProductId = message.ProductId,
            Count = message.Count,
            Address = message.Address
        });

        _logger.LogInformation(
            "Process payment {Value} for product with id {ProductId} was preformed",
            message.PaymentValue,
            message.ProductId);
    }
}
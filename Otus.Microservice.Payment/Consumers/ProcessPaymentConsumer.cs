using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Payment.Consumers;

public class ProcessPaymentConsumer : MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>, IMessageConsumer<ProcessPaymentEvent>
{
    private readonly ILogger<MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>> _logger;
    private readonly IMessagePublisher<BookProductEvent> _bookProductPublisher;

    public ProcessPaymentConsumer(
        ILogger<MessageConsumer<ProcessPaymentEvent, ProcessPaymentRejectEvent>> logger,
        IModel channel,
        IMessagePublisher<ProcessPaymentRejectEvent>? revertMessagePublisher,
        IMessagePublisher<BookProductEvent> bookProductPublisher)
        : base(logger, channel, revertMessagePublisher)

    {
        _logger = logger;
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
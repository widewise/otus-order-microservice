using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Payment.Consumers;

public class BookProductRejectConsumer : MessageConsumer<BookProductRejectEvent, IEvent>, IMessageConsumer<BookProductRejectEvent>
{
    private readonly ILogger<MessageConsumer<BookProductRejectEvent, IEvent>> _logger;
    private readonly IMessagePublisher<ProcessPaymentRejectEvent> _processPaymentRejectPublisher;

    public BookProductRejectConsumer(
        ILogger<MessageConsumer<BookProductRejectEvent, IEvent>> logger,
        IModel channel,
        IMessagePublisher<ProcessPaymentRejectEvent> processPaymentRejectPublisher)
        : base(logger, channel, null)

    {
        _logger = logger;
        _processPaymentRejectPublisher = processPaymentRejectPublisher;
    }

    public override Task ExecuteEventAsync(BookProductRejectEvent message)
    {
        //TODO: reject payment in DB
        _processPaymentRejectPublisher.Send(new ProcessPaymentRejectEvent
        {
            TransactionId = message.TransactionId,
            ProductId = message.ProductId,
        });

        _logger.LogInformation(
            "Booking product with id {ProductId} was rejected",
            message.ProductId);

        return Task.CompletedTask;
    }
}
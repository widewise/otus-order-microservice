using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Store.Consumers;

public class DeliverProductRejectConsumer : MessageConsumer<DeliverProductRejectEvent, IEvent>, IMessageConsumer<DeliverProductRejectEvent>
{
    private readonly ILogger<MessageConsumer<DeliverProductRejectEvent, IEvent>> _logger;
    private readonly IMessagePublisher<BookProductRejectEvent> _bookProductRejectPublisher;

    public DeliverProductRejectConsumer(
        ILogger<MessageConsumer<DeliverProductRejectEvent, IEvent>> logger,
        IModel channel,
        IMessagePublisher<BookProductRejectEvent> bookProductRejectPublisher)
        : base(logger, channel, null)

    {
        _logger = logger;
        _bookProductRejectPublisher = bookProductRejectPublisher;
    }

    public override Task ExecuteEventAsync(DeliverProductRejectEvent message)
    {
        //TODO: reject booking in DB

        _bookProductRejectPublisher.Send(new BookProductRejectEvent
        {
            TransactionId = message.TransactionId,
            ProductId = message.ProductId,
        });

        _logger.LogInformation(
            "Deliver product with id {ProductId} was rejected",
            message.ProductId);

        return Task.CompletedTask;
    }
}
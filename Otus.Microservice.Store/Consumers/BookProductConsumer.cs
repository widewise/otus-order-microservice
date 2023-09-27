using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Store.Consumers;

public class BookProductConsumer : MessageConsumer<BookProductEvent, BookProductRejectEvent>, IMessageConsumer<BookProductEvent>
{
    private readonly ILogger<MessageConsumer<BookProductEvent, BookProductRejectEvent>> _logger;
    private readonly IMessagePublisher<DeliverProductEvent> _deliverProductPublisher;

    public BookProductConsumer(
        ILogger<MessageConsumer<BookProductEvent, BookProductRejectEvent>> logger,
        IModel channel,
        IMessagePublisher<BookProductRejectEvent>? revertMessagePublisher,
        IMessagePublisher<DeliverProductEvent> deliverProductPublisher)
        : base(logger, channel, revertMessagePublisher)
    {
        _logger = logger;
        _deliverProductPublisher = deliverProductPublisher;
    }

    public override async Task ExecuteEventAsync(BookProductEvent message)
    {
        if (message.Count <= 0)
        {
            _logger.LogError("Product count less zero: {CountValue}", message.Count);
            await RejectEventAsync(message);
            return;
        }

        _deliverProductPublisher.Send(new DeliverProductEvent
        {
            TransactionId = message.TransactionId,
            ProductId = message.ProductId,
            Address = message.Address
        });

        _logger.LogInformation(
            "Product with {ProductId} was booked. Count: {ProductCount}",
            message.ProductId,
            message.Count);
    }
}
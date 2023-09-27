using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Delivery.Consumers;

public class DeliverProductConsumer : MessageConsumer<DeliverProductEvent, DeliverProductRejectEvent>,
    IMessageConsumer<DeliverProductEvent>
{
    private readonly ILogger<MessageConsumer<DeliverProductEvent, DeliverProductRejectEvent>> _logger;
    public DeliverProductConsumer(
        ILogger<MessageConsumer<DeliverProductEvent, DeliverProductRejectEvent>> logger,
        IModel channel,
        IMessagePublisher<DeliverProductRejectEvent>? revertMessagePublisher)
        : base(logger, channel, revertMessagePublisher)
    {
        _logger = logger;
    }

    public override async Task ExecuteEventAsync(DeliverProductEvent message)
    {
        if (string.IsNullOrEmpty(message.Address))
        {
            _logger.LogError("Delivery address is empty");
            await RejectEventAsync(message);
            return;
        }

        _logger.LogInformation(
            "Deliver product with id {ProductId} was preformed",
            message.ProductId);
    }
}
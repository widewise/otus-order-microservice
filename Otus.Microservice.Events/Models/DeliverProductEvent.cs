using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Events.Models;

public class DeliverProductEvent : IRejectableEvent
{
    public string TransactionId { get; set; }
    public IEvent GetRejectEvent()
    {
        return new DeliverProductRejectEvent
        {
            TransactionId = TransactionId,
            ProductId = ProductId
        };
    }

    public int ProductId { get; set; }
    public string Address { get; set; } = null!;
}
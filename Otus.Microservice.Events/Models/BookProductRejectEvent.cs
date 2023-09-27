using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Events.Models;

public class BookProductRejectEvent : IRejectableEvent
{
    public IEvent GetRejectEvent()
    {
        return new BookProductRejectEvent
        {
            TransactionId = TransactionId,
            ProductId = ProductId,
        };
    }

    public string TransactionId { get; set; }
    public int ProductId { get; set; }
}
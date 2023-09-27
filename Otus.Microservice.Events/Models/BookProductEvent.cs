using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Events.Models;

public class BookProductEvent : IRejectableEvent
{
    public IEvent GetRejectEvent()
    {
        return new BookProductRejectEvent
        {
            TransactionId = TransactionId,
            ProductId = ProductId
        };
    }

    public string TransactionId { get; set; }
    public int ProductId { get; set; }
    public string Address { get; set; } = null!;
    public int Count { get; set; }
}
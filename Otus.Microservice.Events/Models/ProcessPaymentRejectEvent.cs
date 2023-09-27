using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Events.Models;

public class ProcessPaymentRejectEvent : IEvent
{
    public string TransactionId { get; set; }
    public int ProductId { get; set; }
}
namespace Otus.Microservice.TransportLibrary.Models;

public interface IEvent
{
    public string TransactionId { get; set; }
}
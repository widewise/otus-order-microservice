namespace Otus.Microservice.TransportLibrary.Models;

public interface IRejectableEvent : IEvent
{
    IEvent GetRejectEvent();
}
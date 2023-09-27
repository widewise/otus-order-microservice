namespace Otus.Microservice.TransportLibrary.Services;

public interface IMessagePublisher<TMessage>
{
    void Send<TMessage>(TMessage message);
}
using RabbitMQ.Client;

namespace Otus.Microservice.TransportLibrary.Services;

public interface IMessageConsumer<in TMessage>: IBasicConsumer
{
    Task ExecuteEventAsync(TMessage message);
}
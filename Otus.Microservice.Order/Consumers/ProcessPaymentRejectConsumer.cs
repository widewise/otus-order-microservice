using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Models;
using Otus.Microservice.TransportLibrary.Services;
using RabbitMQ.Client;

namespace Otus.Microservice.Order.Consumers;

public class ProcessPaymentRejectConsumer :
    MessageConsumer<ProcessPaymentRejectEvent, IEvent>,
    IMessageConsumer<ProcessPaymentRejectEvent>
{
    private readonly ILogger<MessageConsumer<ProcessPaymentRejectEvent, IEvent>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ProcessPaymentRejectConsumer(
        ILogger<MessageConsumer<ProcessPaymentRejectEvent, IEvent>> logger,
        IModel channel,
        IServiceScopeFactory scopeFactory)
        : base(logger, channel, null)

    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public override async Task ExecuteEventAsync(ProcessPaymentRejectEvent message)
    {
        await using (var scope = _scopeFactory.CreateAsyncScope())
        {
            var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var order = await dbcontext.Orders.FirstOrDefaultAsync(
                x => x.RequestId == message.TransactionId);
            if (order != null)
            {
                dbcontext.Orders.Remove(order);
                await dbcontext.SaveChangesAsync();
            }
        }

        _logger.LogInformation(
            "Process payment for product with id {ProductId} was rejected",
            message.ProductId);
    }
}
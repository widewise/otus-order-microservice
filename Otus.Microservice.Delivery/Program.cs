using Otus.Microservice.Delivery.Consumers;
using Otus.Microservice.Events;
using Otus.Microservice.Events.Models;
using Otus.Microservice.TransportLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransportCore(builder.Configuration);
builder.Services.AddTransportConsumerWithReject<DeliverProductEvent, DeliverProductRejectEvent, DeliverProductConsumer>(
    EventConstants.StoreExchange,
    EventConstants.DeliveryExchange,
    EventConstants.DeliveryProductEvent);

var app = builder.Build();
app.Services.BuildTransportMap();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
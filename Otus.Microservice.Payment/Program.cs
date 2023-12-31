using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events;
using Otus.Microservice.Events.Models;
using Otus.Microservice.Payment;
using Otus.Microservice.Payment.Consumers;
using Otus.Microservice.TransportLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: true);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (dbConnectionString != null)
{
    // Register database
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(dbConnectionString));
}

builder.Services.AddTransportCore(builder.Configuration);
builder.Services.AddTransportPublisher<NotificationSendEvent>(EventConstants.NotificationExchange);
builder.Services.AddTransportPublisher<BookProductEvent>(EventConstants.StoreExchange);
builder.Services.AddTransportPublisher<ProcessPaymentRejectEvent>(EventConstants.OrderExchange);
builder.Services.AddTransportConsumerWithReject<ProcessPaymentEvent, ProcessPaymentRejectEvent, ProcessPaymentConsumer>(
    EventConstants.OrderExchange,
    EventConstants.PaymentExchange,
    EventConstants.ProcessPaymentEvent);
builder.Services.AddTransportConsumer<BookProductRejectEvent, BookProductRejectConsumer>(
    EventConstants.PaymentExchange,
    EventConstants.StoreProductRejectedEvent);

var app = builder.Build();
app.Services.BuildTransportMap();

var logger = app.Services.GetService<ILogger<Program>>();
logger.LogInformation("DB connection string: {DBConnectionString}", dbConnectionString);

if (dbConnectionString != null)
{
    logger.LogInformation("Start DB migrating ...");

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();

    logger.LogInformation("DB migrated");
}

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
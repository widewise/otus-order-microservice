using Microsoft.EntityFrameworkCore;
using Otus.Microservice.Events;
using Otus.Microservice.Events.Models;
using Otus.Microservice.Notification;
using Otus.Microservice.Notification.Consumers;
using Otus.Microservice.Notification.Models;
using Otus.Microservice.TransportLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection(NotificationSettings.Section));

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
builder.Services.AddTransportConsumer<NotificationSendEvent, NotificationSendConsumer>(
    EventConstants.NotificationExchange,
    EventConstants.NotificationSendEvent);

var app = builder.Build();
app.Services.BuildTransportMap();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
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
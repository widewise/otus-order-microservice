using Otus.Microservice.TransportLibrary.Models;

namespace Otus.Microservice.Events.Models;

public class NotificationSendEvent: IEvent
{
    public string TransactionId { get; set; }
    public NotificationType Type { get; set; }
    public string ToAddress { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public enum NotificationType
{
    Successed = 0,
    Failed = 1
}
namespace Otus.Microservice.Payment.Models;

public class CreateAccount
{
    public string? RequestId { get; set; }
    public string Name { get; set; }
    public string NotificationAddress { get; set; }
}
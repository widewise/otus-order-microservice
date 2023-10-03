using System.ComponentModel.DataAnnotations;

namespace Otus.Microservice.Payment.Models;

public class Account
{
    [Key]
    public long Id { get; set; }
    public string RequestId { get; set; }
    public string Name { get; set; }
    public string NotificationAddress { get; set; }
    public decimal Balance { get; set; }
}
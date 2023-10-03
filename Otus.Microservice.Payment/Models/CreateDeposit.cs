namespace Otus.Microservice.Payment.Models;

public class CreateDeposit
{
    public string? RequestId { get; set; }
    public decimal Value { get; set; }
}
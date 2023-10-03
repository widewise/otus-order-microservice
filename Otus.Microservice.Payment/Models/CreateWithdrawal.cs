namespace Otus.Microservice.Payment.Models;

public class CreateWithdrawal
{
    public string? RequestId { get; set; }
    public decimal Value { get; set; }
}
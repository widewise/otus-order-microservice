using System.ComponentModel.DataAnnotations;

namespace Otus.Microservice.Payment.Models;

public class Transaction
{
    [Key]
    public long Id { get; set; }
    public string RequestId { get; set; }
    public long AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
}

public enum TransactionType
{
    Deposit = 0,
    Withdrawal = 1
}
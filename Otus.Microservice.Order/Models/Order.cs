﻿using System.ComponentModel.DataAnnotations;

namespace Otus.Microservice.Order.Models;

public class Order
{
    [Key]
    public long Id { get; set; }
    public string? RequestId { get; set; }
    public DateTime CreateDate { get; set; }
    public decimal Cost { get; set; }
}
﻿using System.ComponentModel.DataAnnotations;
using Otus.Microservice.Events.Models;

namespace Otus.Microservice.Notification.Models;

public class Notification
{
    [Key]
    public long Id { get; set; }
    public string TransactionId { get; set; }
    public NotificationType Type { get; set; }
    public string ToAddress { get; set; }
    public string FromAddress { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
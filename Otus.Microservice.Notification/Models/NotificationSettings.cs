﻿namespace Otus.Microservice.Notification.Models;

public class NotificationSettings
{
    public static string Section => "NotificationSettings";
    public string FromAddress { get; set; }
}
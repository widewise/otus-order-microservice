namespace Otus.Microservice.Events;

public static class EventConstants
{
    private static readonly string RejectedPostfix = "rejected";
    public static string OrderExchange = "order";
    public static string PaymentExchange = "payment";
    public static string ProcessOperation = "process";
    public static string ProcessPaymentEvent = $"{PaymentExchange}-{ProcessOperation}";
    public static string ProcessPaymentRejectedEvent = $"{ProcessPaymentEvent}-{RejectedPostfix}";

    public static string DeliveryExchange = "delivery";
    public static string DeliveryProductOperation = "product";
    public static string DeliveryProductEvent = $"{DeliveryExchange}-{DeliveryProductOperation}";
    public static string DeliveryProductRejectedEvent = $"{DeliveryProductEvent}-{RejectedPostfix}";

    public static string StoreExchange = "store";
    public static string StoreProductOperation = "product";
    public static string StoreProductEvent = $"{StoreExchange}-{StoreProductOperation}";
    public static string StoreProductRejectedEvent = $"{StoreProductEvent}-{RejectedPostfix}";

    public static string NotificationExchange = "notification";
    public static string NotificationSendOperation = "send";
    public static string NotificationSendEvent = $"{NotificationExchange}-{NotificationSendOperation}";
}
namespace Otus.Microservice.TransportLibrary.Models;

public class MessageTransportSettings
{
    public static string SectionName => "MessageTransport";
    public string Hostname { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}
using System.Text.Json.Serialization;

namespace PikaServer.Domain.Entities;

public class Notification : RootEntityBase
{
    public string Title { get; set; }
    public string Message { get; set; }
    public string? Url { get; set; }
    public NotificationType Type { get; set; }
    [JsonIgnore] public string AccountId { get; set; }
}

public enum NotificationType
{
    Transaction,
    System,
    Other
}
using Newtonsoft.Json;

namespace PikaServer.Infras.Services.Notification;

public class ResponseModel
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
}
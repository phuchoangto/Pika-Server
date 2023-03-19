using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemotePublicKeyResponseData
{
    public RemotePublicKeyResponseData(string key)
    {
        Key = key;
    }

    [JsonPropertyName("key")] public string Key { get; set; }
}
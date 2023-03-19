using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteOAuth2Response
{
    public RemoteOAuth2Response(string idToken, string accessToken, int expiresIn, string tokenType)
    {
        IdToken = idToken;
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
        TokenType = tokenType;
    }

    [JsonPropertyName("id_token")] public string IdToken { get; set; }

    [JsonPropertyName("access_token")] public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")] public string TokenType { get; set; }
}
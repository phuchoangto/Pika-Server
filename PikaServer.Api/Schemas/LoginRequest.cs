namespace PikaServer.Api.Schemas;

public class LoginRequest
{
    public LoginRequest(string username, string password, string deviceId)
    {
        Username = username;
        Password = password;
        DeviceId = deviceId;
    }

    public string Username { get; set; }
    public string Password { get; set; }
    public string DeviceId { get; set; }
}
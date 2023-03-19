namespace PikaServer.Api.Schemas;

public class ChangePasswordRequest
{
    public ChangePasswordRequest(string username, string oldPassword, string newPassword)
    {
        Username = username;
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    public string Username { get; init; }
    public string OldPassword { get; init; }
    public string NewPassword { get; init; }
}
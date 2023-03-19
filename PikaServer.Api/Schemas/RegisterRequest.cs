namespace PikaServer.Api.Schemas;

public class RegisterRequest
{
    public RegisterRequest(string username, string fullName, string email, string identityNumber, string phone,
        string password)
    {
        Username = username;
        FullName = fullName;
        Email = email;
        IdentityNumber = identityNumber;
        Phone = phone;
        Password = password;
    }

    public string Username { get; set; }
    public string FullName { get; init; }
    public string Email { get; init; }
    public string IdentityNumber { get; init; }
    public string Phone { get; init; }
    public string Password { get; init; }
}
namespace PikaServer.Domain.Entities;

public class Account : RootEntityBase
{
    public Account()
    {
    }

    public Account(string username, string fullName, string email, string identityNumber, string phone) : this()
    {
        FullName = fullName;
        Email = email;
        IdentityNumber = identityNumber;
        Phone = phone;
        Username = username;
    }

    public Account(int id, string hdBankUserId, string fullName, string email,
        string identityNumber, string phone, string username) : this(fullName, email, identityNumber,
        phone, username)
    {
        Id = id;
        HdBankUserId = hdBankUserId;
    }

    public string HdBankUserId { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string IdentityNumber { get; set; }
    public string Phone { get; set; }
    public string AccountNo { get; set; }
    public string? DeviceId { get; set; }
    public double PassbookBalance { get; set; }
}
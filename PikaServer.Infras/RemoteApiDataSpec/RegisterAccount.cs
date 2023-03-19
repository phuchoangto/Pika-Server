using System.Text.Json.Serialization;
using PikaServer.Domain.Entities;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteRegisterAccountRequestData
{
    public RemoteRegisterAccountRequestData(string credential, string email, string fullName, string identityNumber,
        string key, string phone)
    {
        Credential = credential;
        Email = email;
        FullName = fullName;
        IdentityNumber = identityNumber;
        Key = key;
        Phone = phone;
    }

    [JsonPropertyName("credential")] public string Credential { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("fullName")] public string FullName { get; set; }

    [JsonPropertyName("identityNumber")] public string IdentityNumber { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("phone")] public string Phone { get; set; }

    public static RemoteRegisterAccountRequestData Create(string credential, string key, Account account)
    {
        return new RemoteRegisterAccountRequestData(credential,
            account.Email,
            account.FullName,
            account.IdentityNumber, key,
            account.Phone);
    }
}

public class RemoteRegisterAccountResponseData
{
    public RemoteRegisterAccountResponseData(string userId)
    {
        UserId = userId;
    }

    [JsonPropertyName("userId")] public string UserId { get; set; }
}

public class RemoteRegisterAccountResult
{
    public string UserId { get; set; }
    public string Message { get; set; }
    public RegisterState State { get; set; }

    public bool IsSuccess => State == RegisterState.Success;

    public static RemoteRegisterAccountResult Success(string userId)
    {
        return new RemoteRegisterAccountResult
        {
            UserId = userId,
            State = RegisterState.Success
        };
    }

    public static RemoteRegisterAccountResult Fail(string message)
    {
        return new RemoteRegisterAccountResult
        {
            Message = message,
            State = RegisterState.Failed
        };
    }
}

public enum RegisterState
{
    Success,
    Failed
}
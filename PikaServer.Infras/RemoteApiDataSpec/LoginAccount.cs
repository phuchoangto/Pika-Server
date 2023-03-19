using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteLoginAccountRequestData
{
    public RemoteLoginAccountRequestData(string credential, string key)
    {
        Credential = credential;
        Key = key;
    }

    [JsonPropertyName("credential")] public string Credential { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }
}

public class RemoteLoginAccountResponseData
{
    public RemoteLoginAccountResponseData(string accountNo)
    {
        AccountNo = accountNo;
    }

    [JsonPropertyName("accountNo")] public string AccountNo { get; set; }
}

public class RemoteLoginAccountResult
{
    public string AccountNo { get; set; }
    public LoginState State { get; set; }
    public string Message { get; set; }

    public bool IsSuccess => State == LoginState.Success;

    public static RemoteLoginAccountResult Success(string accountNo)
    {
        return new RemoteLoginAccountResult
        {
            AccountNo = accountNo,
            State = LoginState.Success
        };
    }

    public static RemoteLoginAccountResult Fail(string message)
    {
        return new RemoteLoginAccountResult
        {
            Message = message,
            State = LoginState.Failed
        };
    }
}

public enum LoginState
{
    Success,
    Failed
}
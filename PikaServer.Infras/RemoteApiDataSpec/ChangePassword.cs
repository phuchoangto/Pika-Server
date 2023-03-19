namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteChangePasswordRequestData : RemoteLoginAccountRequestData
{
    public RemoteChangePasswordRequestData(string credential, string key) : base(credential, key)
    {
    }
}

public class RemoteChangePasswordResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static RemoteChangePasswordResult Success()
    {
        return new RemoteChangePasswordResult
        {
            IsSuccess = true,
            Message = "Success"
        };
    }

    public static RemoteChangePasswordResult Fail(string message)
    {
        return new RemoteChangePasswordResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
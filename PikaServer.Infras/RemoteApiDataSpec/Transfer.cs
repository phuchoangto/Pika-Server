using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteTransferRequestData
{
    public RemoteTransferRequestData(string amount, string description, string fromAcct, string toAcct)
    {
        Amount = amount;
        Description = description;
        FromAcct = fromAcct;
        ToAcct = toAcct;
    }

    [JsonPropertyName("amount")] public string Amount { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("fromAcct")] public string FromAcct { get; set; }

    [JsonPropertyName("toAcct")] public string ToAcct { get; set; }
}

public class RemoteTransferResponseData
{
}

public class RemoteTranserResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    
    public static RemoteTranserResult Success()
    {
        return new RemoteTranserResult
        {
            IsSuccess = true,
        };
    }
    
    public static RemoteTranserResult Fail(string message)
    {
        return new RemoteTranserResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
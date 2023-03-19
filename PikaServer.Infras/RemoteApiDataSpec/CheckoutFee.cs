using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteCheckoutFeeRequestData
{
    public RemoteCheckoutFeeRequestData(string studentId, string amount, string description, string fromAccountNo)
    {
        StudentId = studentId;
        Amount = amount;
        Description = description;
        FromAccountNo = fromAccountNo;
    }

    [JsonPropertyName("sdId")] public string StudentId { get; set; }

    [JsonPropertyName("amount")] public string Amount { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("fromAcct")] public string FromAccountNo { get; set; }
}

public class RemoteCheckoutFeeResponseData
{
}
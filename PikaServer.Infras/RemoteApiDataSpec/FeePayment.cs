using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteFeePaymentRequestData
{
    public RemoteFeePaymentRequestData(string studentId)
    {
        StudentId = studentId;
    }

    [JsonPropertyName("sdId")] public string StudentId { get; set; }
}

public class RemoteFeePaymentResponseData
{
    [JsonPropertyName("payments")] public IEnumerable<FeePayment> Payments { get; set; }

    public class FeePayment
    {
        public FeePayment(string description, string fee)
        {
            Description = description;
            Fee = fee;
        }

        [JsonPropertyName("description")] public string Description { get; set; }

        [JsonPropertyName("fee")] public string Fee { get; set; }
    }
}
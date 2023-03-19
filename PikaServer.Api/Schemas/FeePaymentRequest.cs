namespace PikaServer.Api.Schemas;

public class FeePaymentRequest
{
    public FeePaymentRequest(string sdId, double amount, string fromAaccount, string description)
    {
        SdId = sdId;
        Amount = amount;
        FromAaccount = fromAaccount;
        Description = description;
    }

    public string SdId { get; init; }
    public double Amount { get; init; }
    public string FromAaccount { get; init; }
    public string Description { get; init; }
}
namespace PikaServer.Api.Schemas;

public class TransferRequest
{
    public TransferRequest(string amount, string description, string toAccountNo)
    {
        Amount = amount;
        Description = description;
        ToAccountNo = toAccountNo;
    }

    public string Amount { get; init; }
    public string Description { get; init; }
    public string ToAccountNo { get; init; }
}
namespace PikaServer.Api.Schemas;

public class TransactionHistoryRequest
{
    public TransactionHistoryRequest(string accountNo, DateTime fromDate, DateTime toDate)
    {
        AccountNo = accountNo;
        FromDate = fromDate;
        ToDate = toDate;
    }

    public string AccountNo { get; init; }

    public DateTime FromDate { get; init; }

    public DateTime ToDate { get; init; }
}
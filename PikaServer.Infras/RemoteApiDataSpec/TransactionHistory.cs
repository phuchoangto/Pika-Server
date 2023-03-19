using System.Text.Json.Serialization;

namespace PikaServer.Infras.RemoteApiDataSpec;

public class RemoteTransactionHistoryRequestData
{
    public RemoteTransactionHistoryRequestData(string accountNo, string fromDate, string toDate)
    {
        AccountNo = accountNo;
        FromDate = fromDate;
        ToDate = toDate;
    }

    [JsonPropertyName("acctNo")] public string AccountNo { get; set; }

    [JsonPropertyName("fromDate")] public string FromDate { get; set; }

    [JsonPropertyName("toDate")] public string ToDate { get; set; }
}

public class RemoteTransactionHistoryResponseData
{
    public RemoteTransactionHistoryResponseData()
    {
    }

    public RemoteTransactionHistoryResponseData(IEnumerable<Transaction> transactionList)
    {
        TransactionList = transactionList;
    }

    [JsonPropertyName("transHis")] public IEnumerable<Transaction> TransactionList { get; set; }

    public class Transaction
    {
        public Transaction(string transDesc, string transDate, string transAmount)
        {
            TransDesc = transDesc;
            TransDate = transDate;
            TransAmount = transAmount;
        }

        [JsonPropertyName("transDesc")] public string TransDesc { get; set; }

        [JsonPropertyName("transDate")] public string TransDate { get; set; }

        [JsonPropertyName("transAmount")] public string TransAmount { get; set; }
    }
}
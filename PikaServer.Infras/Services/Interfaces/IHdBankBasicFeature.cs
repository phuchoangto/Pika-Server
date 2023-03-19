using PikaServer.Common.HdBankHttpApiBase;
using PikaServer.Infras.RemoteApiDataSpec;

namespace PikaServer.Infras.Services.Interfaces;

public interface IHdBankBasicFeature
{
    Task<double?> GetBalanceAsync(string accountNo, CancellationToken cancellationToken = default);

    Task<RemoteTranserResult> TransferAsync(string amount, string description, string fromAccNo, string toAccNo,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<RemoteTransactionHistoryResponseData.Transaction>> GetTransactionHistAsync(string accountNo,
        DateTime fromDate,
        DateTime toDate, CancellationToken cancellationToken = default);

    Task<IEnumerable<RemoteFeePaymentResponseData.FeePayment>> GetFeePaymentAsync(string sdId,
        CancellationToken cancellationToken = default);

    Task<AuditResponse> CheckoutFeeAsync(string sdId, double amount, string description, string fromAccNo,
        CancellationToken cancellationToken = default);
}
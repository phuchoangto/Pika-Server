using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PikaServer.Common.Extensions;
using PikaServer.Common.HdBankHttpApiBase;
using PikaServer.Common.Utils;
using PikaServer.Infras.Constants;
using PikaServer.Infras.Helpers;
using PikaServer.Infras.RemoteApiDataSpec;
using PikaServer.Infras.Services.Interfaces;

namespace PikaServer.Infras.Services.ApiFeature;

public class HdBankBasicFeature : IHdBankBasicFeature
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HdBankBasicFeature> _logger;

    public HdBankBasicFeature(ILogger<HdBankBasicFeature> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(HttpClientNameConstants.HdBank);
    }

    public async Task<double?> GetBalanceAsync(string accountNo, CancellationToken cancellationToken = default)
    {
        // prepare body
        var body = new HdBankRemoteApiRequest<RemoteBalanceRequestData>(new RemoteBalanceRequestData(accountNo));

        // send req
        var httpResponse = await _httpClient.PostAsync("balance", body.AsJsonContent(), cancellationToken);

        var response =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteBalanceResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(response);
        
        _logger.LogError("Get balance fail due to: {message}", PikaJsonConvert.SerializeObject(response.Data));

        if (!response!.Response.IsResponseCodeSuccess())
        {
            _logger.LogError("Get balance fail due to: {message}", PikaJsonConvert.SerializeObject(response.Response));
            return null;
        }

        return double.Parse(response.Data.Amount);
    }

    public async Task<RemoteTranserResult> TransferAsync(string amount, string description, string fromAccNo, string toAccNo,
        CancellationToken cancellationToken = default)
    {
        // prepare body
        var reqBody = new HdBankRemoteApiRequest<RemoteTransferRequestData>(new RemoteTransferRequestData(
            amount, description, fromAccNo, toAccNo));

        // send req
        var httpResponse = await _httpClient.PostAsync("transfer", reqBody.AsJsonContent(), cancellationToken);

        // parse result
        var response =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteTransferResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(response);
        ProcessTransferLog(response, reqBody);
        
        if (!response!.Response.IsResponseCodeSuccess())
        {
            _logger.LogError("Transfer fail due to: {message}", PikaJsonConvert.SerializeObject(response.Response));
            return RemoteTranserResult.Fail(response.Response.ResponseMessage);
        }

        return RemoteTranserResult.Success();
    }

    public async Task<IEnumerable<RemoteTransactionHistoryResponseData.Transaction>> GetTransactionHistAsync(
        string accountNo, DateTime fromDate, DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        // prepare body
        var fromDateFormat = fromDate.ToString("ddMMyyyy");
        var toDateFormat = toDate.ToString("ddMMyyyy");
        var reqBody = new HdBankRemoteApiRequest<RemoteTransactionHistoryRequestData>(
            new RemoteTransactionHistoryRequestData(
                accountNo, fromDateFormat, toDateFormat));

        // send req
        var httpResponse = await _httpClient.PostAsync("tranhis", reqBody.AsJsonContent(), cancellationToken);

        var response = await httpResponse.Content
            .ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteTransactionHistoryResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(response);

        return response!.Data.TransactionList;
    }

    public async Task<IEnumerable<RemoteFeePaymentResponseData.FeePayment>> GetFeePaymentAsync(string sdId,
        CancellationToken cancellationToken = default)
    {
        // prepare body
        var reqBody =
            new HdBankRemoteApiRequest<RemoteFeePaymentRequestData>(new RemoteFeePaymentRequestData(sdId));

        // send req
        var httpResponse = await _httpClient.PostAsync("getpayment", reqBody.AsJsonContent(), cancellationToken);

        var response =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteFeePaymentResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(response);

        return response!.Data.Payments;
    }

    public async Task<AuditResponse> CheckoutFeeAsync(string sdId, double amount, string description, string fromAccNo,
        CancellationToken cancellationToken = default)
    {
        var reqBody = new HdBankRemoteApiRequest<RemoteCheckoutFeeRequestData>(new RemoteCheckoutFeeRequestData(
            sdId, amount.ToString(CultureInfo.InvariantCulture), description, fromAccNo));

        var httpResponse = await _httpClient.PostAsync("payment", reqBody.AsJsonContent(), cancellationToken);

        var response = await httpResponse.Content
            .ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteCheckoutFeeResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(response);

        return response!.Response;
    }

    private void ProcessTransferLog(HdBankRemoteApiResponse<RemoteTransferResponseData>? response,
        HdBankRemoteApiRequest<RemoteTransferRequestData> reqBody)
    {
        if (!response!.Response.IsResponseCodeSuccess())
            _logger.LogError("Transfer fail due to: {message}", PikaJsonConvert.SerializeObject(response.Response));

        _logger.LogInformation("Transfer request success: {content}", PikaJsonConvert.SerializeObject(reqBody));
    }
}
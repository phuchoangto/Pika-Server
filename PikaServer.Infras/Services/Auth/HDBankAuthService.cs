using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PikaServer.Common.Extensions;
using PikaServer.Common.HdBankHttpApiBase;
using PikaServer.Common.Utils;
using PikaServer.Domain.Entities;
using PikaServer.Infras.AppSettings;
using PikaServer.Infras.Constants;
using PikaServer.Infras.Helpers;
using PikaServer.Infras.RemoteApiDataSpec;
using PikaServer.Infras.Services.Credential;
using PikaServer.Infras.Services.Interfaces;

namespace PikaServer.Infras.Services.Auth;

public class HdBankAuthService : IHdBankAuthService
{
    private const string OAuth2GrantType = "refresh_token";

    private readonly HdBankApiSetting _hdBankApiSetting;
    private readonly HdBankCredential _hdBankCredential;

    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HdBankAuthService> _logger;
    private readonly RsaCredentialHelper _rsaCredentialHelper;

    public HdBankAuthService(IOptions<HdBankApiSetting> hdBankApiSettingOption,
        ILogger<HdBankAuthService> logger,
        IHttpClientFactory httpClientFactory,
        HdBankCredential hdBankCredential,
        RsaCredentialHelper rsaCredentialHelper)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _hdBankCredential = hdBankCredential;
        _rsaCredentialHelper = rsaCredentialHelper;
        _hdBankApiSetting = hdBankApiSettingOption.Value;

        _httpClient = _httpClientFactory.CreateClient(HttpClientNameConstants.HdBank);
    }

    public async Task<RemoteOAuth2Response> OAuth2Async(CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientNameConstants.HdBankAuth);
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _hdBankApiSetting.ClientId),
            new KeyValuePair<string, string>("grant_type", OAuth2GrantType),
            new KeyValuePair<string, string>("refresh_token", _hdBankApiSetting.RefreshToken)
        });

        var httpResponse = await httpClient.PostAsync("/oauth2/token", formContent, cancellationToken);
        if (httpResponse.StatusCode != HttpStatusCode.OK)
            _logger.LogError("Perform OAuth2 fail with error {statusCode}, {error}",
                httpResponse.StatusCode, await httpResponse.Content.ReadAsStringAsync(cancellationToken));

        var responseData =
            await httpResponse.Content.ReadFromJsonAsync<RemoteOAuth2Response>(
                cancellationToken: cancellationToken);
        if (responseData is not null)
        {
            _logger.LogInformation("Perform OAuth2 success: {data}", PikaJsonConvert.SerializeObject(responseData));

            _hdBankCredential.SetAccessToken(responseData.AccessToken);
            _hdBankCredential.SetIdToken(responseData.IdToken);

            return responseData;
        }

        _logger.LogError("Cannot read HDBank OAuth2 response data due to null");
        throw new Exception("Service Unavailable");
    }

    public async Task<RemoteRegisterAccountResult> RegisterAccountAsync(Account account, string password,
        CancellationToken cancellationToken = default)
    {
        var reqBody = new HdBankRemoteApiRequest<RemoteRegisterAccountRequestData>(
            RemoteRegisterAccountRequestData.Create(
                _rsaCredentialHelper.CreateCredential(account.Username, password),
                _hdBankCredential.RsaPublicKey,
                account));

        var httpResponse =
            await _httpClient.PostAsync("register", reqBody.AsJsonContent(), cancellationToken);

        var responseData =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteRegisterAccountResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(responseData);

        if (!responseData!.Response.IsResponseCodeSuccess())
        {
            _logger.LogError("Register account fail due to: {message}",
                PikaJsonConvert.SerializeObject(responseData.Response));
            return RemoteRegisterAccountResult.Fail(responseData.Response.ResponseMessage);
        }

        _logger.LogInformation("Register account success: {data}", PikaJsonConvert.SerializeObject(responseData!.Data));
        return string.IsNullOrEmpty(responseData?.Data.UserId)
            ? RemoteRegisterAccountResult.Fail(responseData.Response.ResponseMessage)
            : RemoteRegisterAccountResult.Success(responseData.Data.UserId);
    }

    public async Task<RemoteLoginAccountResult> LoginAccountAsync(Account account, string password,
        CancellationToken cancellationToken = default)
    {
        // prepare body
        var reqBody = new HdBankRemoteApiRequest<RemoteLoginAccountRequestData>(new RemoteLoginAccountRequestData(
            _rsaCredentialHelper.CreateCredential(account.Username, password),
            _hdBankCredential.RsaPublicKey));

        // send req
        var httpResponse = await _httpClient.PostAsync("login", reqBody.AsJsonContent(), cancellationToken);

        var responseData =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemoteLoginAccountResponseData>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(responseData);


        if (!responseData!.Response.IsResponseCodeSuccess())
        {
            _logger.LogError("Register account fail due to: {message}",
                PikaJsonConvert.SerializeObject(responseData.Response));
            return RemoteLoginAccountResult.Fail(responseData.Response.ResponseMessage);
        }

        // check login
        return string.IsNullOrEmpty(responseData?.Data.AccountNo)
            ? RemoteLoginAccountResult.Fail(responseData.Response.ResponseMessage)
            : RemoteLoginAccountResult.Success(responseData.Data.AccountNo);
    }

    public async Task<RemoteChangePasswordResult> ChangePassword(string username, string oldPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        // prepare body
        var reqBody = new HdBankRemoteApiRequest<RemoteChangePasswordRequestData>(new RemoteChangePasswordRequestData(
            _rsaCredentialHelper.CreateCredential(username, oldPassword, newPassword),
            _hdBankCredential.RsaPublicKey));

        // send req
        var httpResponse = await _httpClient.PostAsync("change_password", reqBody.AsJsonContent(), cancellationToken);

        // validate response
        var responseData =
            await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<object>>(
                cancellationToken: cancellationToken);

        EnsureHdBankApiResponseHelper.ThrowIfNull(responseData);

        if (!responseData!.Response.IsResponseCodeSuccess())
        {
            _logger.LogError("Register account fail due to: {message}",
                PikaJsonConvert.SerializeObject(responseData.Response));
            return RemoteChangePasswordResult.Fail(responseData.Response.ResponseMessage);
        }

        return RemoteChangePasswordResult.Success();
    }
}
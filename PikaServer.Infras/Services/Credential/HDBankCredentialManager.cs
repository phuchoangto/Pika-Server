using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PikaServer.Common.HdBankHttpApiBase;
using PikaServer.Infras.Constants;
using PikaServer.Infras.RemoteApiDataSpec;
using PikaServer.Infras.Services.Interfaces;

namespace PikaServer.Infras.Services.Credential;

public class HdBankCredentialManager : IHdBankCredentialManager
{
    private readonly IHdBankAuthService _authService;
    private readonly HdBankCredential _credential;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HdBankCredentialManager> _logger;

    public HdBankCredentialManager(HdBankCredential credential, IHdBankAuthService authService,
        IHttpClientFactory httpClientFactory, ILogger<HdBankCredentialManager> logger)
    {
        _credential = credential;
        _authService = authService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }


    public string GetPublicKey()
    {
        if (string.IsNullOrEmpty(_credential.RsaPublicKey)) Task.FromResult(ClaimPublicKeyAsync());

        return _credential.RsaPublicKey!;
    }

    public async Task ClaimPublicKeyAsync(CancellationToken cancellationToken = default)
    {
        await _authService.OAuth2Async(cancellationToken);

        var httpClient = _httpClientFactory.CreateClient(HttpClientNameConstants.HdBank);

        var httpResponse = await httpClient.GetAsync("get_key", cancellationToken);
        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError("{error}", httpResponse.RequestMessage?.ToString());
            _logger.LogError("Claim public_key fail due to: status_code: {code}, msg: {error}",
                httpResponse.StatusCode,
                await httpResponse.Content.ReadAsStringAsync(cancellationToken));
            throw new Exception("Service Unavailable");
        }

        try
        {
            var responseData =
                await httpResponse.Content.ReadFromJsonAsync<HdBankRemoteApiResponse<RemotePublicKeyResponseData>>(
                    cancellationToken: cancellationToken);

            var publicKey = responseData?.Data.Key;

            if (string.IsNullOrEmpty(publicKey)) throw new NullReferenceException();

            _logger.LogInformation("Claim public_key success: {pubKey}", publicKey);
            _credential.SetRsaPublicKey(publicKey);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{message}", e.Message);
            throw new Exception(e.Message);
        }
    }
}
using Microsoft.Extensions.Logging;
using PikaServer.Infras.Services.Credential;

namespace PikaServer.Infras.DelegateHandler;

public class HdBankHttpHandler : DelegatingHandler
{
    private readonly HdBankCredential _credential;
    private readonly ILogger<HdBankHttpHandler> _logger;

    public HdBankHttpHandler(HdBankCredential credential, ILogger<HdBankHttpHandler> logger)
    {
        _credential = credential;
        _logger = logger;
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("access-token", _credential.IdToken);
        await HandleLogBodyContentBeforeSend(request, cancellationToken);

        var httpResponse = await base.SendAsync(request, cancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
            // log process if not success
            await HandleLogError(cancellationToken, httpResponse);

        return httpResponse;
    }

    private async Task HandleLogBodyContentBeforeSend(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var method = request.Method.ToString();

        if (!method.Equals("POST")) return;

        var bodyContent = request.Content is null
            ? ""
            : await request.Content?.ReadAsStringAsync(cancellationToken)!;

        _logger.LogInformation("Request to Uri: \"{Uri}\", Body: {Body}",
            request.RequestUri?.AbsoluteUri,
            bodyContent);
    }

    private async Task HandleLogError(CancellationToken cancellationToken, HttpResponseMessage httpResponse)
    {
        var errorMsg = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError("{Request}", httpResponse.RequestMessage?.ToString());
        _logger.LogError("Register account fail due to: status_code: {code}, msg: {message}",
            httpResponse.StatusCode,
            errorMsg);
    }
}
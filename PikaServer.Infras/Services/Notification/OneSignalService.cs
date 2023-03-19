using Microsoft.Extensions.Configuration;
using PikaServer.Infras.Services.Interfaces;
using OneSignalApi.Api;
using OneSignalApi.Client;
using OneSignalApi.Model;

namespace PikaServer.Infras.Services.Notification;

public class OneSignalService : IOneSignalService
{
    private readonly IConfiguration _configuration;
    private readonly DefaultApi _oneSignalApi;

    public OneSignalService(IConfiguration configuration)
    {
        _configuration = configuration;
        var config = new Configuration
        {
            AccessToken = _configuration["OneSignal:AccessToken"],
            BasePath = _configuration["OneSignal:BasePath"],
        };
        _oneSignalApi = new DefaultApi(config);
    }

    public Task<CreateNotificationSuccessResponse> SendNotification(string title, string message, string url, List<String> deviceIds)
    {
        var notification = new OneSignalApi.Model.Notification(appId: _configuration["OneSignal:AppId"])
        {
            Contents = new StringMap(en: message),
            Headings = new StringMap(en: title),
            Url = url,
            IncludePlayerIds = deviceIds,
        };
        
        return _oneSignalApi.CreateNotificationAsync(notification);
    }
}
using OneSignalApi.Model;

namespace PikaServer.Infras.Services.Interfaces;

public interface IOneSignalService
{
    public Task<CreateNotificationSuccessResponse> SendNotification(string title, string message, string url, List<String> deviceIds);
}
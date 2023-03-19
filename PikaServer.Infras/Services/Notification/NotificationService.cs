using System.Net.Http.Headers;
using CorePush.Google;
using Microsoft.Extensions.Options;

namespace PikaServer.Infras.Services.Notification;

public interface INotificationService
{
    Task<ResponseModel> SendNotification(NotificationModel notificationModel);
}

public class NotificationService : INotificationService
{
    private readonly NotificationSetting _notificationSetting;

    public NotificationService(IOptions<NotificationSetting> settings)
    {
        _notificationSetting = settings.Value;
    }

    public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
    {
        var response = new ResponseModel();
        try
        {
            if (notificationModel.IsAndroiodDevice)
            {
                /* FCM Sender (Android Device) */
                var settings = new FcmSettings
                {
                    SenderId = "733640586662",
                    ServerKey = "AAAAqtBi9aY:APA91bGi47K8J5SPmtUNtdNGY2myuBjxEpzbV09u3HnSnonnori6SnWJKkyowAD8Ms0yD624k3auy3UDGqZHdQJBjYUU_GKfdCu0u7FMkggBWcwXWjcndmPIU-geK-7YcCoGx5G5TBzR"
                };

                var httpClient = new HttpClient();

                var authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                var deviceToken = notificationModel.DeviceId;

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var dataPayload = new GoogleNotification.DataPayload();
                dataPayload.Title = notificationModel.Title;
                dataPayload.Body = notificationModel.Body;

                var notification = new GoogleNotification();
                notification.Data = dataPayload;
                notification.Notification = dataPayload;

                var fcm = new FcmSender(settings, httpClient);
                var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                if (fcmSendResponse.IsSuccess())
                {
                    response.IsSuccess = true;
                    response.Message = "Notification sent successfully";
                    return response;
                }

                response.IsSuccess = false;
                response.Message = fcmSendResponse.Results[0].Error;
                return response;
            }

            /* Code here for APN Sender (iOS Device) */
            //var apn = new ApnSender(apnSettings, httpClient);
            //await apn.SendAsync(notification, deviceToken);
            return response;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
            return response;
        }
    }
}
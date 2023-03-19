using PikaServer.Common.HdBankHttpApiBase;

namespace PikaServer.Infras.Helpers;

public static class EnsureHdBankApiResponseHelper
{
    public static void ThrowIfNull<T>(HdBankRemoteApiResponse<T>? response)
    {
        if (response is null || response.Response is null) throw new NullReferenceException();
    }
}
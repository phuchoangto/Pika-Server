using PikaServer.Infras.Services.Interfaces;

namespace PikaServer.ServiceExtensions;

public static class PostStartupExtensions
{
    public static IServiceProvider PostExecution(this IServiceProvider sp)
    {
        var scope = sp.CreateScope();
        ClaimClientPublicKey(scope);

        return sp;
    }

    private static void ClaimClientPublicKey(IServiceScope scope)
    {
        var services = scope.ServiceProvider.GetRequiredService<IHdBankCredentialManager>();

        services.ClaimPublicKeyAsync();
    }
}
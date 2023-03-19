using PikaServer.Domain.Entities;
using PikaServer.Infras.RemoteApiDataSpec;

namespace PikaServer.Infras.Services.Interfaces;

public interface IHdBankAuthService
{
    Task<RemoteOAuth2Response> OAuth2Async(CancellationToken cancellationToken = default);

    Task<RemoteRegisterAccountResult> RegisterAccountAsync(Account account, string password,
        CancellationToken cancellationToken = default);

    Task<RemoteLoginAccountResult> LoginAccountAsync(Account account, string password,
        CancellationToken cancellationToken = default);

    Task<RemoteChangePasswordResult> ChangePassword(string username, string oldPassword, string newPassword,
        CancellationToken cancellationToken = default);
}
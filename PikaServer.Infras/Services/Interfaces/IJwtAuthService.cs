using System.Security.Claims;
using PikaServer.Domain.Entities;

namespace PikaServer.Infras.Services.Interfaces;

public interface IJwtAuthService
{
    string CreateJwtAccessToken(Account account);

    Task<Account?> GetAccountFromClaimsAsync(IEnumerable<Claim> claims);

    IEnumerable<Claim> ValidateJwtToken(string jwtToken);
}
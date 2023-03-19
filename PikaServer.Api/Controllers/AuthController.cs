using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PikaServer.Api.Schemas;
using PikaServer.Domain.Entities;
using PikaServer.Infras.Services.Interfaces;
using PikaServer.Persistence.Internal.Abstracts;

namespace PikaServer.Api.Controllers;

public class AuthController : ApiV1ControllerBase
{
    private readonly IHdBankAuthService _hdBankAuthService;
    private readonly IHdBankBasicFeature _hdBankBasicFeature;
    private readonly IHdBankCredentialManager _hdBankCredentialManager;
    private readonly IJwtAuthService _jwtAuthService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(IHdBankAuthService hdBankAuthService, IHdBankCredentialManager hdBankCredentialManager,
        IHdBankBasicFeature hdBankBasicFeature, IJwtAuthService jwtAuthService, IUnitOfWork unitOfWork)
    {
        _hdBankAuthService = hdBankAuthService;
        _hdBankCredentialManager = hdBankCredentialManager;
        _hdBankBasicFeature = hdBankBasicFeature;
        _jwtAuthService = jwtAuthService;
        _unitOfWork = unitOfWork;
    }

    /*[HttpPost("oauth2/token")]
    [AllowAnonymous]
    public async Task<IActionResult> OAuth2(CancellationToken cancellationToken = default)
    {
        return Ok(await _hdBankAuthService.OAuth2Async(cancellationToken));
    }*/

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var accountInDb =
            await _unitOfWork.Account.FindOneAsync(x => x.Username == request.Username || x.Email == request.Email);
        if (accountInDb != null) return BadRequest("Username or email already exists");
        var account = new Account(request.Username, request.FullName, request.Email, request.IdentityNumber,
            request.Phone);
        var registerResult = await _hdBankAuthService.RegisterAccountAsync(account, request.Password);
        if (!registerResult.IsSuccess) return BadRequest(new { registerResult.Message });
        account.HdBankUserId = registerResult.UserId;
        var loginResult = await _hdBankAuthService.LoginAccountAsync(account, request.Password);
        if (!loginResult.IsSuccess) return BadRequest();
        account.AccountNo = loginResult.AccountNo;
        var newAcc = await _unitOfWork.Account.InsertAsync(account);
        await _unitOfWork.CommitAsync();
        return Ok(new
        {
            Account = newAcc,
            Token = _jwtAuthService.CreateJwtAccessToken(newAcc)
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginResult =
            await _hdBankAuthService.LoginAccountAsync(new Account { Username = request.Username }, request.Password);
        if (!loginResult.IsSuccess) return BadRequest(new { loginResult.Message });

        var account = await _unitOfWork.Account.FindOneAsync(x => x.Username == request.Username);
        if (account == null) return BadRequest(new { Message = "Account not found" });

        account.DeviceId = request.DeviceId;
        var result = await _unitOfWork.Account.UpdateAsync(account);
        
        await _unitOfWork.CommitAsync();
        
        return Ok(new
        {
            Account = account,
            Token = _jwtAuthService.CreateJwtAccessToken(account)
        });
    }

    [HttpPost("change_password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var changePasswordResult =
            await _hdBankAuthService.ChangePassword(request.Username, request.OldPassword, request.NewPassword);
        if (!changePasswordResult.IsSuccess) return BadRequest(new { changePasswordResult.Message });
        return Ok();
    }
}
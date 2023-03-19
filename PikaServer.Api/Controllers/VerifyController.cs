using Microsoft.AspNetCore.Mvc;
using PikaServer.Infras.Services.Interfaces;

namespace PikaServer.Api.Controllers;

public class VerifyController : ApiV1ControllerBase
{
    private readonly ITwilioVerificationService _twilioVerificationService;

    public VerifyController(ITwilioVerificationService twilioVerificationService)
    {
        _twilioVerificationService = twilioVerificationService;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequest request)
    {
        var result = await _twilioVerificationService.StartVerificationAsync(request.Phone, request.Channel);
        return Ok(result);
    }
    
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        var result = await _twilioVerificationService.CheckVerificationAsync(request.Phone, request.Code);
        return Ok(result);
    }
}

public class VerifyCodeRequest
{
    public string Phone { get; set; }
    public string Code { get; set; }
}

public class SendVerificationCodeRequest
{
    public string Phone { get; set; }
    public string Channel { get; set; }
}
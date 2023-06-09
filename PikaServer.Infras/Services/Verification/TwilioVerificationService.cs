﻿using Microsoft.Extensions.Configuration;
using PikaServer.Infras.Services.Interfaces;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace PikaServer.Infras.Services.Verification;

public class TwilioVerificationService : ITwilioVerificationService
{
    private readonly IConfiguration _configuration;

    public TwilioVerificationService(IConfiguration configuration)
    {
        _configuration = configuration;
        TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
    }

    public async Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel)
    {
        try
        {
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: channel,
                pathServiceSid: _configuration["Twilio:VerificationSid"]
            );
            return new VerificationResult(verificationResource.Sid);
        }
        catch (TwilioException e)
        {
            return new VerificationResult(new List<string>{e.Message});
        }
    }

    public async Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code)
    {
        try
        {
            var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _configuration["Twilio:VerificationSid"]
            );
            return verificationCheckResource.Status.Equals("approved") ?
                new VerificationResult(verificationCheckResource.Sid) :
                new VerificationResult(new List<string>{"Wrong code. Try again."});
        }
        catch (TwilioException e)
        {
            return new VerificationResult(new List<string>{e.Message});
        }
    }
}
using PikaServer.Infras.Services.Verification;

namespace PikaServer.Infras.Services.Interfaces;

public interface ITwilioVerificationService
{
    Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel);
    Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code);
}
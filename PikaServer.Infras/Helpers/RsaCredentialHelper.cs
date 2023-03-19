using System.Security.Cryptography;
using System.Text;
using PikaServer.Infras.Services.Credential;
using XC.RSAUtil;

namespace PikaServer.Infras.Helpers;

public class RsaCredentialHelper
{
    private const int DwKeySize = 1024;
    private static string? _xmlKey;

    private readonly HdBankCredential _credential;

    public RsaCredentialHelper(HdBankCredential credential)
    {
        _credential = credential;
    }

    public string CreateCredential(string username, string password, string? newPassword = null)
    {
        var dataByteToEncrypt = Encoding.UTF8.GetBytes(CombinePlainContent(username, password, newPassword));

        var rsa = new RSACryptoServiceProvider(DwKeySize);
        rsa.FromXmlString(GetKeyXmlFormat());

        var encrypted = rsa.Encrypt(dataByteToEncrypt, false);
        return Convert.ToBase64String(encrypted);
    }

    public static string CombinePlainContent(string username, string password, string? newPassword = null)
    {
        return string.IsNullOrEmpty(newPassword)
            ? $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}"
            : $"{{\"username\":\"{username}\",\"oldPass\":\"{password}\",\"newPass\":\"{newPassword}\"}}";
    }

    private string GetKeyXmlFormat()
    {
        if (string.IsNullOrEmpty(_xmlKey)) _xmlKey = RsaKeyConvert.PublicKeyPemToXml(_credential.RsaPublicKey);

        return _xmlKey;
    }
}
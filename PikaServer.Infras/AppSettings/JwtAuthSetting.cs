namespace PikaServer.Infras.AppSettings;

public class JwtAuthSetting
{
	public string Secret { get; set; } = null!;
	public uint ExpirationInMinutes { get; set; } = 120;
}

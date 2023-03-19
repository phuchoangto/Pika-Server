using System.Text.Json.Serialization;

namespace PikaServer.Common.HdBankHttpApiBase;

public class HdBankRemoteApiRequest<TData>
{
	public HdBankRemoteApiRequest(TData data)
	{
		Data = data;
		Request = new AuditRequest();
	}

	[JsonPropertyName("data")] public TData Data { get; set; }

	[JsonPropertyName("request")] public AuditRequest Request { get; set; }

	public class AuditRequest
	{
		[JsonPropertyName("requestId")] public string RequestId { get; set; } = Guid.NewGuid().ToString();

		[JsonPropertyName("requestTime")]
		public string RequestTime { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
	}
}

using System.Text.Json.Serialization;

namespace PikaServer.Common.HdBankHttpApiBase;

public class HdBankRemoteApiResponse<TData>
{
	public HdBankRemoteApiResponse(TData data, AuditResponse response)
	{
		Data = data;
		Response = response;
	}

	[JsonPropertyName("data")] public TData Data { get; set; }

	[JsonPropertyName("response")] public AuditResponse Response { get; set; }
}

public class AuditResponse
{
	public AuditResponse(string responseId, string responseCode, string responseMessage, string responseTime)
	{
		ResponseId = responseId;
		ResponseCode = responseCode;
		ResponseMessage = responseMessage;
		ResponseTime = responseTime;
	}

	[JsonPropertyName("responseId")] public string ResponseId { get; set; }

	[JsonPropertyName("responseCode")] public string ResponseCode { get; set; }

	[JsonPropertyName("responseMessage")] public string ResponseMessage { get; set; }

	[JsonPropertyName("responseTime")] public string ResponseTime { get; set; }

	public bool IsResponseCodeSuccess()
	{
		return ResponseCode.Equals(HdBankResponseCodeSpec.Success);
	}
}

using System.Text;
using System.Text.Json;
using PikaServer.Common.Utils;

namespace PikaServer.Common.Extensions;

public static class Extensions
{
	public static StringContent AsJsonContent(this object obj)
	{
		return new StringContent(JsonSerializer.Serialize(obj, PikaJsonOptions.BuildEscapeNonAscii()),
			Encoding.UTF8, "application/json");
	}
}

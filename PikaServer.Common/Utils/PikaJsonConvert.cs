using Newtonsoft.Json;

namespace PikaServer.Common.Utils;

public class PikaJsonConvert
{
	public static string SerializeObject(object obj, JsonSerializerSettings? setting = null)
	{
		return setting is null
			? JsonConvert.SerializeObject(obj, PikaJsonOptions.SnakeCase)
			: JsonConvert.SerializeObject(obj, setting);
	}
}

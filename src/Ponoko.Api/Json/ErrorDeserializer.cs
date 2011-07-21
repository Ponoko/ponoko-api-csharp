using System;
using Newtonsoft.Json;
using Ponoko.Api.Core;

namespace Ponoko.Api.Json {
	public static class ErrorDeserializer {
		public static Error Deserialize(String json) {
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var payload = new Deserializer().Deserialize(json);
			return JsonConvert.DeserializeObject<Error>(payload["error"].ToString(), settings);
		}
	}
}
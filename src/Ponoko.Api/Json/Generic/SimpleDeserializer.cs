using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ponoko.Api.Json.Generic {
	public static class SimpleDeserializer<T> {
		public static T Deserialize(String json) {
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				Converters = new List<JsonConverter> { new DateTimeReader() },
			};

			return JsonConvert.DeserializeObject<T>(json, settings);
		}
	}
}

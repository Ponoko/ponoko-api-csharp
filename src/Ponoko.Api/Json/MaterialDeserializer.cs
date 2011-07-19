using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ponoko.Api.Core;

namespace Ponoko.Api.Json {
	public static class MaterialDeserializer {
		public static Material Deserialize(String json) {
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Error,
				Converters = new List<JsonConverter> { new DateTimeReader() },
			};

			return JsonConvert.DeserializeObject<Material>(json, settings);
		}
	}
}

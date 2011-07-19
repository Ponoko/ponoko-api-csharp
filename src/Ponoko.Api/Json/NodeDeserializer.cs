using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ponoko.Api.Core;

namespace Ponoko.Api.Json {
	public static class NodeDeserializer {
		public static Node Deserialize(String json) {
			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Error,
          		Converters = new List<JsonConverter> { new DateTimeReader() },
			};

			return JsonConvert.DeserializeObject<Node>(json, settings);
		}
	}
}
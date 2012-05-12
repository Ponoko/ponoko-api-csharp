using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Ponoko.Api.Core;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Json {
	public static class MaterialDeserializer {
		public static Material Deserialize(String json) {
			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Ignore,
          		Converters = new List<JsonConverter> { new DateTimeReader() }
			};

			var result = JsonConvert.DeserializeObject<Material>(json, settings);
			result.Attributes.Add(GetExtraAttributes(json));
			return result;
		}

		private static NameValueCollection GetExtraAttributes(String json) {
			var result = new NameValueCollection();

			var skip = new List<String> {"key", "name", "type", "updated_at", "dimensions"};	

			var material = new Deserializer().Deserialize(json);

			foreach (var attribute in material.Properties()) {
				un.less(() => skip.Contains(attribute.Name), () => 
					result.Add(attribute.Name, material.Value<String>(attribute.Name))
				);
			}

			return result;
		} 
	}
}
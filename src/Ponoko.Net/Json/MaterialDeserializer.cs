using System;
using System.Collections.Generic;
using System.Globalization;
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

	public class DateTimeReader : JsonConverter {
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			throw new NotImplementedException("This type does not support writing.");
		}

		public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer) {
			return DateTime.ParseExact(
				reader.Value.ToString(), 
				"yyyy/MM/dd HH:mm:ss +0000",
				CultureInfo.InvariantCulture
			);
		}

		public override bool CanConvert(Type objectType) {
			return objectType.Equals(typeof(DateTime));
		}
	}
}

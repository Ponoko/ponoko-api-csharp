using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Ponoko.Api.Json {
	public class DateTimeReader : JsonConverter {
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			throw new NotImplementedException("This type does not support writing.");
		}

		public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer) {
			return DateTime.ParseExact(
				reader.Value.ToString(), 
				"yyyy/MM/dd HH:mm:ss K",
				CultureInfo.InvariantCulture, 
				DateTimeStyles.AdjustToUniversal
			);
		}

		public override bool CanConvert(Type objectType) {
			return objectType.Equals(typeof(DateTime));
		}
	}
}
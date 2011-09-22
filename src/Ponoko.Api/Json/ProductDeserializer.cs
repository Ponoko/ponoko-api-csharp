using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ponoko.Api.Core.Product;

namespace Ponoko.Api.Json {
	public static class ProductDeserializer {
		public static Product Deserialize(String json) {
			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Error,
          		Converters = new List<JsonConverter> { new DateTimeReader() }
			};

			return TryDeserialize(json, settings);
		}

		private static Product TryDeserialize(String json, JsonSerializerSettings settings) {
			try {
				return JsonConvert.DeserializeObject<Product>(json, settings);
			} catch (Exception e) {
				throw new Exception(String.Format("Failed to deserialize: \n\n{0}", json), e);
			}
		}
	}
}
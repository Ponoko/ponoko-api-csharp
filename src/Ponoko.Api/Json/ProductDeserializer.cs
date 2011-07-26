using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;

namespace Ponoko.Api.Json {
	public static class ProductDeserializer {
		public static Product Deserialize(String json) {
			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Error,
          		Converters = new List<JsonConverter> { new DateTimeReader() }
			};

			return JsonConvert.DeserializeObject<Product>(json, settings);
		}
	}
}
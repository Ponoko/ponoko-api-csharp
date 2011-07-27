using System;
using Newtonsoft.Json;
using Ponoko.Api.Core.Shipping;

namespace Ponoko.Api.Json {
	public static class ShippingOptionsDeserializer {
		public static ShippingOptions Deserialize(String json) {
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var payload = new Deserializer().Deserialize(json);
			return JsonConvert.DeserializeObject<ShippingOptions>(payload["shipping_options"].ToString(), settings);
		}
	}
}
using System;
using Newtonsoft.Json;
using Ponoko.Api.Core.Orders;

namespace Ponoko.Api.Json {
	public static class OrderDeserializer {
		public static Order Deserialize(String json) {
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			
			return JsonConvert.DeserializeObject<Order>(json, settings);
		}	
	}
}
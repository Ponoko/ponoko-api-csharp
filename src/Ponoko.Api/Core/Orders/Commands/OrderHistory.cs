using System;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Orders.Commands {
	public class OrderHistory : Domain {
		public OrderHistory(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public Order[] FindAll() {
			var response = _internet.Get(Map("/orders"));
			
			return Deserialize(ReadAll(response));
		}

		public Order Find(String key) {
			return GetAndDeserialize(Map("/orders/{0}", key));
		}

		public Order Status(String key) {
			return GetAndDeserialize(Map("/orders/status/{0}", key));
		}

		private Order GetAndDeserialize(Uri uri) {
			var response = _internet.Get(uri);

			var json = ReadAll(response);

			var theNode = new Deserializer().Deserialize(json)["order"].ToString();

			return OrderDeserializer.Deserialize(theNode);
		}

		private Order[] Deserialize(String json) {
			return OrderListDeserializer.Deserialize(json);
		}
	}
}
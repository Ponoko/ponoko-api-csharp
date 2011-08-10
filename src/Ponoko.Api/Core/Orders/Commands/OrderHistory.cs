using System;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Orders.Commands {
	public class OrderHistory : Domain {
		public OrderHistory(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Order[] FindAll() {
			var response = _internet.Get(Map("/orders"));
			
			return Deserialize(ReadAll(response));
		}

		private Order[] Deserialize(String json) {
			return OrderListDeserializer.Deserialize(json);
		}		
	}
}
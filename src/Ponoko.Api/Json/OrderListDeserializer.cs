using System;
using Ponoko.Api.Core.Orders;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Json {
	public static class OrderListDeserializer {
		public static Order[] Deserialize(String json) {
			return new ListDeserializer<Order>(OrderDeserializer.Deserialize, "orders").
				Deserialize(json);
		}
	}
}

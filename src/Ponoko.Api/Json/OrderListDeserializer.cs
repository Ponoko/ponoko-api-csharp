using System;
using System.Collections.Generic;
using Ponoko.Api.Core.Orders;

namespace Ponoko.Api.Json {
	public static class OrderListDeserializer {
		public static Order[] Deserialize(String json) {
			var theList = new Deserializer().Deserialize(json);
			var result = new List<Order>();

			foreach (var orderJson in theList["orders"].Children()) {
				result.Add(OrderDeserializer.Deserialize(orderJson.ToString()));
			}

			return result.ToArray();
		}
	}
}

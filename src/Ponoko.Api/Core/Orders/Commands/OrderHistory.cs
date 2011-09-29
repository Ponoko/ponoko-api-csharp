using System;
using System.Collections.Generic;
using System.Net;
using Ponoko.Api.Core.Shipping;
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

		public Order Create(String reference, Option shippingOption, NameAndAddress shipTo, ProductShippingInfo product) {
			var uri = Map("/orders");

			var payload = new Payload {
				new Field { Name = "ref", Value = reference },                                    	
				new Field { Name = "shipping_option_code", Value = shippingOption.Code },                                    	
			};

			payload.AddRange(Format(product));
			payload.AddRange(Format(shipTo));
			
			var response = Post(uri, payload);

			if (response.StatusCode != HttpStatusCode.OK)
				throw Error("Failed to create order", response);

			var json = ReadAll(response);

			var theOrderNode = new Deserializer().Deserialize(json)["order"];

			return OrderDeserializer.Deserialize(theOrderNode.ToString());
		}

		private IEnumerable<Field> Format(NameAndAddress shipTo) {
			return new List<Field> {
				new Field { Name = "billing_address[first_name]", Value = shipTo.FirstName},			                           	
				new Field { Name = "billing_address[last_name]", Value = shipTo.LastName},			                           	
				new Field { Name = "billing_address[address_line_1]", Value = shipTo.LineOne},			                           	
				new Field { Name = "billing_address[address_line_2]", Value = shipTo.LineTwo},			                           	
				new Field { Name = "billing_address[city]", Value = shipTo.City},			                           	
				new Field { Name = "billing_address[state]", Value = shipTo.State},			                           	
				new Field { Name = "billing_address[zip_or_postal_code]", Value = shipTo.ZipOrPostalCode},			                           	
				new Field { Name = "billing_address[country]", Value = shipTo.Country},			                           	
				new Field { Name = "billing_address[phone_number]", Value = shipTo.Phone},	
				new Field { Name = "delivery_address[first_name]", Value = shipTo.FirstName},			                           	
				new Field { Name = "delivery_address[last_name]", Value = shipTo.LastName},			                           	
				new Field { Name = "delivery_address[address_line_1]", Value = shipTo.LineOne},			                           	
				new Field { Name = "delivery_address[address_line_2]", Value = shipTo.LineTwo},			                           	
				new Field { Name = "delivery_address[city]", Value = shipTo.City},			                           	
				new Field { Name = "delivery_address[state]", Value = shipTo.State},			                           	
				new Field { Name = "delivery_address[zip_or_postal_code]", Value = shipTo.ZipOrPostalCode},			                           	
				new Field { Name = "delivery_address[country]", Value = shipTo.Country},			                           	
				new Field { Name = "delivery_address[phone_number]", Value = shipTo.Phone},			                           	
			};
		}

		private IEnumerable<Field> Format(ProductShippingInfo info) {
			return new List<Field> {
           		new Field {Name = "products[][key]", Value = info.Key},
           		new Field {Name = "products[][quantity]", Value = info.Quantity.ToString()}
			};
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
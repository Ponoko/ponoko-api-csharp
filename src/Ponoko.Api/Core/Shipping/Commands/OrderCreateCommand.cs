using System;
using System.Collections.Generic;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Shipping.Commands {
	public class OrderCreateCommand : Domain {
		public OrderCreateCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Order Create(String reference, Option shippingOption, NameAndAddress shipTo, ProductShippingInfo product) {
			var uri = Map("/orders");

			var parameters = new List<Parameter> {
				new Parameter { Name = "ref", Value = reference },                                    	
				new Parameter { Name = "shipping_option_code", Value = shippingOption.Code },                                    	
			};

			parameters.AddRange(Format(product));
			parameters.AddRange(Format(shipTo));
			
			var response = _internet.Post(uri, new Payload(parameters));

			Console.WriteLine(response.StatusCode);
			Console.WriteLine(new Deserializer().Deserialize(ReadAll(response)));

			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception("Server returned " + response.StatusCode);

			return new Order();
		}

		private IEnumerable<Parameter> Format(NameAndAddress shipTo) {
			return new List<Parameter> {
				new Parameter { Name = "billing_address[first_name]", Value = shipTo.FirstName},			                           	
				new Parameter { Name = "billing_address[last_name]", Value = shipTo.LastName},			                           	
				new Parameter { Name = "billing_address[address_line_1]", Value = shipTo.LineOne},			                           	
				new Parameter { Name = "billing_address[address_line_2]", Value = shipTo.LineTwo},			                           	
				new Parameter { Name = "billing_address[city]", Value = shipTo.City},			                           	
				new Parameter { Name = "billing_address[state]", Value = shipTo.State},			                           	
				new Parameter { Name = "billing_address[zip_or_postal_code]", Value = shipTo.ZipOrPostalCode},			                           	
				new Parameter { Name = "billing_address[country]", Value = shipTo.Country},			                           	
				new Parameter { Name = "billing_address[phone_number]", Value = shipTo.Phone},	
				new Parameter { Name = "delivery_address[first_name]", Value = shipTo.FirstName},			                           	
				new Parameter { Name = "delivery_address[last_name]", Value = shipTo.LastName},			                           	
				new Parameter { Name = "delivery_address[address_line_1]", Value = shipTo.LineOne},			                           	
				new Parameter { Name = "delivery_address[address_line_2]", Value = shipTo.LineTwo},			                           	
				new Parameter { Name = "delivery_address[city]", Value = shipTo.City},			                           	
				new Parameter { Name = "delivery_address[state]", Value = shipTo.State},			                           	
				new Parameter { Name = "delivery_address[zip_or_postal_code]", Value = shipTo.ZipOrPostalCode},			                           	
				new Parameter { Name = "delivery_address[country]", Value = shipTo.Country},			                           	
				new Parameter { Name = "delivery_address[phone_number]", Value = shipTo.Phone},			                           	
			};
		}

		private IEnumerable<Parameter> Format(IEnumerable<ProductShippingInfo> products) {
			var result = new List<Parameter>();

			foreach (var info in products) {
				result.AddRange(Format(info));
			}

			return result;
		}

		private IEnumerable<Parameter> Format(ProductShippingInfo info) {
			return new List<Parameter> {
           		new Parameter {Name = "products[][key]", Value = info.Key},
           		new Parameter {Name = "products[][quantity]", Value = info.Quantity.ToString()}
			};
		}
	}
}
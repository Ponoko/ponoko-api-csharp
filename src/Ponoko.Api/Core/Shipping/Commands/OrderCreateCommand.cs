﻿using System;
using System.Collections.Generic;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Core.Shipping.Commands {
	public class OrderCreateCommand : Domain {
		public OrderCreateCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Order Create(String reference, Option shippingOption, NameAndAddress shipTo, ProductShippingInfo product) {
			var uri = Map("/orders");

			var fields = new List<Field> {
				new Field { Name = "ref", Value = reference },                                    	
				new Field { Name = "shipping_option_code", Value = shippingOption.Code },                                    	
			};

			fields.AddRange(Format(product));
			fields.AddRange(Format(shipTo));
			
			var response = _internet.Post(uri, new FormUrlEncoded(), new Payload(fields));

			Console.WriteLine(response.StatusCode);
			Console.WriteLine(new Deserializer().Deserialize(ReadAll(response)));

			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception("Server returned " + response.StatusCode);

			return new Order();
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

		private IEnumerable<Field> Format(IEnumerable<ProductShippingInfo> products) {
			var result = new List<Field>();

			foreach (var info in products) {
				result.AddRange(Format(info));
			}

			return result;
		}

		private IEnumerable<Field> Format(ProductShippingInfo info) {
			return new List<Field> {
           		new Field {Name = "products[][key]", Value = info.Key},
           		new Field {Name = "products[][quantity]", Value = info.Quantity.ToString()}
			};
		}
	}
}
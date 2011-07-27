using System;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Shipping.Commands {
	public class FindCommand : Domain {
		public FindCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public ShippingOptions For(Product.Product product) {
			Uri uri = Map(
				"/orders/shipping-options?" + 
				"products[][key]={0}&" + 
				"products[][quantity]=1&" + 
				"delivery_address[address_line_1]=27%20Dixon%20Street&" + 
				"delivery_address[address_line_2]=Te%20Aro&"+ 
				"delivery_address[city]=Wellington&" + 
				"delivery_address[state]=na&" + 
				"delivery_address[zip_or_postal_code]=6021&" +
				"delivery_address[country]=New%20Zealand", 
				product.Key
			);

			var response = _internet.Get(uri);
			return ShippingOptionsDeserializer.Deserialize(ReadAll(response));
		}	
	}
}
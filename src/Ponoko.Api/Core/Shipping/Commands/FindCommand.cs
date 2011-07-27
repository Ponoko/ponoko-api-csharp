using System;
using System.Text;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Shipping.Commands {
	public class FindCommand : Domain {
		public FindCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public ShippingOptions For(Address to, params ProductShippingInfo[] products) {
			var productsString = new StringBuilder();

			foreach (var info in products) {
				productsString.AppendFormat(
					"products[][key]={0}&products[][quantity]={1}&", 
					UrlEncode(info.Key), UrlEncode(info.Quantity)
				);
			}

			var address = new StringBuilder();

			address.AppendFormat(
				"delivery_address[address_line_1]={0}&" + 
				"delivery_address[address_line_2]={1}&"+ 
				"delivery_address[city]={2}&" + 
				"delivery_address[state]={3}&" + 
				"delivery_address[zip_or_postal_code]={4}&" +
				"delivery_address[country]={5}", 
				UrlEncode(to.LineOne), 
				UrlEncode(to.LineTwo),
				UrlEncode(to.City),
				UrlEncode(to.State),
				UrlEncode(to.ZipOrPostalCode),
				UrlEncode(to.Country)
			);

			Uri uri = Map(
				"/orders/shipping-options?" + 
				"{0}{1}", 
				productsString,
				address
			);

			var response = _internet.Get(uri);
			return ShippingOptionsDeserializer.Deserialize(ReadAll(response));
		}	

		private String UrlEncode(Object what) {
			if (what == null) return null;
			return Uri.EscapeDataString(what.ToString());
		}
	}
}
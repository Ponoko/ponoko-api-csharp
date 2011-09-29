using System;
using System.Collections.Generic;
using System.Text;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Shipping.Repositories {
	public class ShippingOptionsRepository : Domain {
		public ShippingOptionsRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public ShippingOptions For(Address to, params ProductShippingInfo[] products) {
			var uri = Map(
				"/orders/shipping-options?{0}{1}", 
				Format(products),
				Format(to)
			);

			return Deserialize(_internet.Get(uri));
		}

		private ShippingOptions Deserialize(Response response) {
			return ShippingOptionsDeserializer.Deserialize(ReadAll(response));
		}

		private String Format(IEnumerable<ProductShippingInfo> products) {
			var buffer = new StringBuilder();

			foreach (var info in products) {
				buffer.Append(Format(info));
			}

			return buffer.ToString();
		}

		private String Format(ProductShippingInfo info) {
			return String.Format(
				"products[][key]={0}&products[][quantity]={1}&", 
				UrlEncode(info.Key), UrlEncode(info.Quantity)
			);
		}

		private String Format(Address to) {
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

			return address.ToString();
		}

		private String UrlEncode(Object what) {
			if (what == null) return null;
			return Uri.EscapeDataString(what.ToString());
		}
	}
}
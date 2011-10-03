using System;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class HardwareRepository : ProductRepository  {
		public HardwareRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public Product Add(String productKey, String sku, Int32 quantity) {
			var uri = Map("/products/{0}/hardware", productKey);

			var payload = new Payload {
				{"sku", sku},
			    {"quantity", quantity}
			};

			using (var response = Post(uri, payload)) {
				return Deserialize(response);
			}
		}

		public Product Remove(String productKey, String sku) {
			var uri = Map("/products/{0}/hardware", productKey);

			var payload = new Payload { {"sku", sku} };

			using (var response = Post(uri, payload)) {
				return Deserialize(response);
			}
		}
	}
}
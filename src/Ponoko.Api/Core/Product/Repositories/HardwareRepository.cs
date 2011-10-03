using System;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class HardwareRepository : Domain {
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

		private Product Deserialize(Response response) {
			if (response.StatusCode != HttpStatusCode.OK)
				throw Error("Unexpected status returned.", response);

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}
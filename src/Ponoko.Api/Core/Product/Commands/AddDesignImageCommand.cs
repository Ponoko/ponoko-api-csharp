using System;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class AddDesignImageCommand : Domain {
		public AddDesignImageCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }

		public Product Add(String productKey, FileInfo file) {
			var uri = Map("/products/{0}/design-images", productKey);

			var payload = new Payload { { "design_images[][uploaded_data]", new DataItem(file, "image/gif") } };

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		public Stream Get(String productKey, String filename) {
			var uri = Map("/products/{0}/design-images/download?filename={1}", productKey, filename);

			return Get(uri).Open(); 
		}

		public Product Remove(String productKey, String filename) {
			var uri = Map("/products/{0}/design-images/destroy?filename={1}", productKey, filename);

			using (var response = Get(uri)) {
				return Deserialize(response);
			}
		}

		private Product Deserialize(Response response) {
			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception("Invalid status returned");

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}
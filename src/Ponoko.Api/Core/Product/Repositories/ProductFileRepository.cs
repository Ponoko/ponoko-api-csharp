using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class ProductFileRepository : Domain {
		private readonly string _resource;

		public ProductFileRepository(TheInternet internet, String baseUrl, String resource)
			: base(internet, baseUrl) {
			_resource = resource;
		}

		public Product Add(String productKey, params File[] files) {
			var uri = Map("/products/{0}/{1}", productKey, _resource);

			var payload = ToPayload(files);

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Payload ToPayload(IEnumerable<File> designImages) {
			var payload = new Payload();

			foreach (var designImage in designImages) {
				payload.Add(
					"assembly_instructions[][uploaded_data]",
					new DataItem(new FileInfo(designImage.FullName), designImage.ContentType)
				);
			}

			return payload;
		}

		public Stream Get(String productKey, String filename) {
			var uri = Map("/products/{0}/{1}/download?filename={2}", productKey, _resource, filename);

			return Get(uri).Open();
		}

		private Product Deserialize(Response response) {
			if (response.StatusCode != HttpStatusCode.OK)
				throw Error("Invalid status returned", response);

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}
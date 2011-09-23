using System;
using System.IO;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class DesignImageAddCommand : Domain {
		public DesignImageAddCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }

		public Product Add(String product, FileInfo file) {
			var uri = Map("/products/{0}/design-images", product);

			var payload = new Payload { { "design_images[][uploaded_data]", new DataItem(file, "image/gif") } };

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		public Byte[] Get(String product, String filename) {
			var uri = Map("/products/{0}/design-images/download?filename={1}", product, filename);

			using (var response = Get(uri)) {
				var length = Int32.Parse(response.Header("Content-length"));
				return ReadAll(response.Open(), length);
			}
		}

		public Product Remove(String product, String filename) {
			var uri = Map("/products/{0}/design-images/destroy?filename={1}", product, filename);

			using (var response = Get(uri)) {
				return Deserialize(response);
			}
		}

		private Product Deserialize(Response response) {
			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}

		private byte[] ReadAll(Stream input, Int32 length) {
			const Int32 BUFFER_SIZE = 1024 * 10;

			using (var output = new MemoryStream(length)) {
				var buffer = new Byte[BUFFER_SIZE];
				var bytesRead = 0;

				while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
					output.Write(buffer, 0, bytesRead);
				}

				return output.GetBuffer();
			}
		}
	}
}
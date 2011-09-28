using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class DesignImage {
		public FileInfo FileInfo { get; private set; }
		public string ContentType { get; private set; }

		public DesignImage(FileInfo fileInfo, String contentType) {
			FileInfo = fileInfo;
			ContentType = contentType;
		}
	}
	
	public class DesignImagesRepository : Domain {
		public DesignImagesRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }

		public Product Add(String productKey, params DesignImage[] designImages) {
			var uri = Map("/products/{0}/design-images", productKey);

			var payload = ToPayload(designImages);

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Payload ToPayload(IEnumerable<DesignImage> designImages) {
			var payload = new Payload();

			foreach (var designImage in designImages) {
				payload.Add(
					"design_images[][uploaded_data]", 
					new DataItem(designImage.FileInfo, designImage.ContentType)
				);
			}

			return payload;
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
				throw Error("Invalid status returned", response);

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}
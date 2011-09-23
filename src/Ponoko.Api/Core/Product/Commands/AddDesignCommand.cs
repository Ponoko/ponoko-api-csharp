using System;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class AddDesignCommand : Domain {
		public AddDesignCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Product Add(String productKey, Design design) {
			var uri = Map("/products/{0}/add-design", productKey);
			return Submit(uri, design);
		}

		public Product Update(String productKey, Design design) {
			var uri = Map("/products/{0}/update-design/{1}", productKey, design.Key);

			var payload = new Payload();

			payload.Add("ref",			design.Reference);
			payload.Add("quantity",		design.Quantity);
			payload.Add("material_key",	design.MaterialKey);

			return Run(uri, payload);
		}

		public Product Update(String productKey, Design design, FileInfo newFile) {
			var uri = Map("/products/{0}/update-design/{1}", productKey, design.Key);
			
			design.Filename = newFile.FullName;
			
			return Submit(uri, design);
		}

		private Product Submit(Uri uri, Design design) {
			var payload = new Payload();

			payload.Add("ref",				design.Reference);
			payload.Add("filename",			Path.GetFileName(design.Filename));
			payload.Add("uploaded_data",	new DataItem(new FileInfo(design.Filename), "xxx"));
			payload.Add("quantity",			design.Quantity);
			payload.Add("material_key",		design.MaterialKey);

			return Run(uri, payload);
		}

		private Product Run(Uri uri, Payload payload) {
			using (var response = MultipartPost(uri, payload)) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error("Failed to update design", response);
			}
		}

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}
	}
}

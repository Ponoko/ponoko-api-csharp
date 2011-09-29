using System;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.Product.Commands {
	public class DesignRepository : Domain {
		public DesignRepository(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Product Add(String productKey, Design design) {
			var uri = Map("/products/{0}/add-design", productKey);
			return Submit(uri, design);
		}

		public Product Update(String productKey, Design design) {
			var uri = Map("/products/{0}/update-design/{1}", productKey, design.Key);

			var payload = new Payload {
				{"ref"			, design.Reference},
			    {"quantity"		, design.Quantity},
			    {"material_key"	, design.MaterialKey}
			};

			return Run(uri, payload);
		}

		public Product Update(String productKey, Design design, FileInfo newFile) {
			var uri = Map("/products/{0}/update-design/{1}", productKey, design.Key);
			
			design.Filename = newFile.FullName;
			
			return Submit(uri, design);
		}

		public Product Delete(String productKey, String designKey) {
			var uri = Map("/products/{0}/delete-design/{1}", productKey, designKey);

			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw Error("Delete failed", response);
				});

				var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
				return ProductDeserializer.Deserialize(json);
			}
		}

		private Product Submit(Uri uri, Design design) {
			var payload = new Payload {
				{"ref"				, design.Reference},
			    {"filename"			, Path.GetFileName(design.Filename)},
			    {"uploaded_data"	, new DataItem(new FileInfo(design.Filename), "xxx")},
			    {"quantity"			, design.Quantity},
			    {"material_key"		, design.MaterialKey}
			};

			return Run(uri, payload);
		}

		private Product Run(Uri uri, Payload payload) {
			using (var response = MultipartPost(uri, payload)) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error("Failed to update or add design", response);
			}
		}

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}
	}
}

using System;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

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

			payload.Fields.Add(new Field {Name = "ref",				Value = design.Reference});
			payload.Fields.Add(new Field {Name = "quantity",		Value = design.Quantity});
			payload.Fields.Add(new Field {Name = "material_key",	Value = design.MaterialKey});

			return Run(uri, payload);
		}

		public Product Update(String productKey, Design design, FileInfo newFile) {
			var uri = Map("/products/{0}/update-design/{1}", productKey, design.Key);
			
			design.Filename = newFile.FullName;
			
			return Submit(uri, design);
		}

		private Product Submit(Uri uri, Design design) {
			var payload = new Payload();

			payload.Fields.Add(new Field {Name = "ref",				Value = design.Reference});
			payload.Fields.Add(new Field {Name = "filename",		Value = Path.GetFileName(design.Filename)});
			payload.Fields.Add(new Field {
			    Name = "uploaded_data",	
			    Value = new DataItem("uploaded_data", new FileInfo(design.Filename), "xxx")
			});
			payload.Fields.Add(new Field {Name = "quantity",		Value = design.Quantity});
			payload.Fields.Add(new Field {Name = "material_key",	Value = design.MaterialKey});

			return Run(uri, payload);
		}

		private Product Run(Uri uri, Payload payload) {
			using (var response = MultipartPost(uri, payload)) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error(response);
			}
		}

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}

		private Exception Error(Response response) {
			var json = ReadAll(response);
			var theError = TryDeserialize(json);

			return new Exception(String.Format(
				"Failed to update design. The server returned status {0} ({1}), and error message: \"{2}\"", 
				response.StatusCode, 
				(Int32)response.StatusCode, 
				theError
			));
		}
	}
}

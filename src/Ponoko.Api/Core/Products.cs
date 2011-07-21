using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Ponoko.Api.Core.IO;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core {
	public class Products : Domain {
		private readonly ReadonlyFileSystem _fileSystem;

		public Products(TheInternet internet, String baseUrl, ReadonlyFileSystem fileSystem) : base(internet, baseUrl) {
			_fileSystem = fileSystem;
		}

		public Product Save(String name, Design design) {
			Validate(design);

			var parameters = new NameValueCollection {
				{"name"						, name}, 
				{"designs[][ref]"			, design.Reference},
				{"designs[][filename]"		, design.Filename},
				{"designs[][quantity]"		, design.Quantity.ToString()},
				{"designs[][material_key]"	, design.MaterialKey},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(design.Filename), "xxx"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile))) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error(response);
			}
		}

		private void Validate(Design design) {
			if (null == design)
				throw new ArgumentException("Cannot create a product without at least one Design.", "design");
			
			if (null == design.Filename)
				throw new ArgumentException("Cannot create a product unless the Design has a file.", "design");

			var theDesignFileExistsOnDisk = _fileSystem.Exists(design.Filename);

			un.less(() => theDesignFileExistsOnDisk, () => {
				throw new FileNotFoundException(
					"Cannot create a product unless the Design has a file that exists on disk. " + 
					"Unable to find file \"" + design.Filename + "\""
				);
			});
		}

		private Exception Error(Response response) {
			var json = ReadAll(response);

			var theError = ErrorDeserializer.Deserialize(json);

			return new Exception(String.Format(
				"Failed to save product. The server returned status {0} ({1}), and error message: \"{2}\"", 
				response.StatusCode, 
				(Int32)response.StatusCode, 
				theError
			));
		}

		private Product Deserialize(Response response) {
			var payload = new Deserializer().Deserialize(ReadAll(response));

			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Error,
          		Converters = new List<JsonConverter> { new DateTimeReader() }
			};

			return JsonConvert.DeserializeObject<Product>(payload["product"].ToString(), settings);
		}

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, payload); }
	}
}

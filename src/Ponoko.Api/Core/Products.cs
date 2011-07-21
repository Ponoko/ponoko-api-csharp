using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
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

		public Product Create(String name, Design design) {
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

		public void Delete(string id) {
			var uri = Map("/products/delete/{0}", id);

			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw new Exception("Delete failed");
				});

				Verify(response);
			}
		}

		private void Validate(Design design) {
			if (null == design)
				throw new ArgumentException("Cannot create a product without at least one Design.", "design");
			
			if (null == design.Filename)
				throw new ArgumentException("Cannot create a product unless the Design has a file.", "design");

			var theDesignFileExistsOnDisk = FileExists(design);

			un.less(theDesignFileExistsOnDisk, () => {
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

		private Boolean FileExists(Design design) { return _fileSystem.Exists(design.Filename); }

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}

		private void Verify(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response));

			var deleted = json.Value<String>("deleted");
			var wasDeletedOkay = (deleted == "true");

			un.less(wasDeletedOkay, () => {
				throw new Exception(String.Format(
					"Delete failed. Expected the deleted flag to be true. but it was \"{0}\".", 
					deleted
			     ));
			});
		}

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, payload); }
	}
}
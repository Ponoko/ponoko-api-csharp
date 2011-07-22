using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class ProductFinder : Domain {
		public ProductFinder(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Product[] FindAll() {
			var response = GetAndEnsure(Map("/products"));

			var theList = new Deserializer().Deserialize(ReadAll(response));

			var result = new List<Product>();

			foreach (var productJson in theList["products"].Children()) {
				result.Add(Deserialize(productJson));
			}

			return result.ToArray();
		}

		public Product Find(String id) {
			var response = GetAndEnsure(Map("/products/{0}", id));

			Product aProductThatDoesNotExist = null;

			if (response.StatusCode == HttpStatusCode.NotFound)
				return aProductThatDoesNotExist;

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];

			return Deserialize(productJson);
		}

		public Boolean Exists(String id) {
			var response = _internet.Get(Map("/products/{0}", id));
	
			EnsureAuthorized(response);

			return response.StatusCode == HttpStatusCode.OK;
		}

		private Product Deserialize(JToken productJson) {
			return ProductDeserializer.Deserialize(productJson.ToString());
		}

		private Response GetAndEnsure(Uri uri) {
			var response = _internet.Get(uri);

			EnsureAuthorized(response);

			return response;
		}

		private void EnsureAuthorized(Response response) {
			if (response.StatusCode == HttpStatusCode.Unauthorized) {
				var message = String.Format(
					"Authorization failed. " +
					"The server returned status {0} ({1}).", 
					response.StatusCode, 
					(Int32)response.StatusCode
				);

				throw new Exception(message);
			}
		}
	}
}

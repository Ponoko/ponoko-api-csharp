using System;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Core.Product.Commands {
	public class UpdateCommand : Domain {
		private readonly ProductValidator _validator;

		public UpdateCommand(TheInternet internet, String baseUrl, ProductValidator validator) : base(internet, baseUrl) {
			_validator = validator;
		}

		public Product Update(String productKey, ProductSeed seed) {
			Validate(seed);	

			var payload = ToPayload(seed);

			var uri = Map("/products/{0}/update", productKey);

			using (var response = Post(uri, payload)) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error(response);
			}
		}

		private void Validate(ProductSeed seed) {
			_validator.Validate(seed);
		}

		private Exception Error(Response response) {
			var json = ReadAll(response);
			var theError = TryDeserialize(json);

			return new Exception(String.Format(
				"Failed to save product. The server returned status {0} ({1}), and error message: \"{2}\"", 
				response.StatusCode, 
				(Int32)response.StatusCode, 
				theError
			));
		}

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}

		private Payload ToPayload(ProductSeed seed) {
			var payload = new Payload();

			payload.Fields.Add(new Field {Name = "name",		Value = seed.Name});
			payload.Fields.Add(new Field {Name = "description", Value = seed.Notes});
			payload.Fields.Add(new Field {Name = "ref",			Value = seed.Reference});
			
			return payload;
		}

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, new FormUrlEncoded(), payload); }
	}
}
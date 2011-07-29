using System;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

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

				throw Error("Failed to update product", response);
			}
		}

		private void Validate(ProductSeed seed) {
			_validator.Validate(seed);
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
	}
}
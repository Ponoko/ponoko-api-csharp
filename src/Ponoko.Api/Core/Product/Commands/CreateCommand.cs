using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Core.Product.Commands {
	public class CreateCommand : Domain {
		private readonly ProductValidator _validator;

		public CreateCommand(TheInternet internet, String baseUrl, ProductValidator validator) : base(internet, baseUrl) {
			_validator = validator;
		}

		public Product Create(ProductSeed seed, params Design[] designs) {
			Validate(seed);	
			Validate(designs);

			var payload = ToPayload(seed, designs);

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, payload)) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error(response);
			}
		}

		private void Validate(Design[] designs) { _validator.Validate(designs); }
		private void Validate(ProductSeed seed) { _validator.Validate(seed); }

		private Payload ToPayload(ProductSeed seed, params Design[] designs) {
			var payload = new Payload();
			payload.Fields.Add(new Field {Name = "name", Value = seed.Name});
			payload.Fields.Add(new Field {Name = "notes", Value = seed.Notes});
			payload.Fields.Add(new Field {Name = "ref", Value = seed.Reference});
			
			foreach (var design in designs) {
				payload.Fields.AddRange(ToFields(design));

				payload.Fields.Add(new Field {
				    Value = new DataItem("designs[][uploaded_data]", new FileInfo(design.Filename), "xxx")
				});
			}
			
			return payload;
		}

		private IEnumerable<Field> ToFields(Design design) {
			return new List<Field> {
             	new Field {Name = "designs[][ref]",				Value = design.Reference},
             	new Field {Name = "designs[][filename]",		Value = Path.GetFileName(design.Filename)},
             	new Field {Name = "designs[][quantity]",		Value = design.Quantity.ToString()},
             	new Field {Name = "designs[][material_key]",	Value = design.MaterialKey}
			};
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

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, new MultipartFormData(), payload); }
	}
}
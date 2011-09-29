using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class CreateCommand : Domain {
		private readonly ProductValidator _validator;

		public CreateCommand(TheInternet internet, String baseUrl, ProductValidator validator) : base(internet, baseUrl) {
			_validator = validator;
		}

		public Product Create(ProductSeed seed, params Design[] designs) {
			Validate(seed);	
			Validate(designs);

			var uri = Map("/products");

			using (var response = MultipartPost(uri, ToPayload(seed, designs))) {
				if (response.StatusCode == HttpStatusCode.OK)
					return Deserialize(response);

				throw Error("Failed to create product", response);
			}
		}

		private void Validate(Design[] designs) { _validator.Validate(designs); }
		private void Validate(ProductSeed seed) { _validator.Validate(seed); }

		private Payload ToPayload(ProductSeed seed, params Design[] designs) {
			var payload = new Payload {
          		{ "name",	seed.Name},
          		{ "notes",	seed.Notes},
          		{ "ref",	seed.Reference}
			};

			foreach (var design in designs) {
				payload.AddRange(ToFields(design));

				payload.Add(
					"designs[][uploaded_data]",
				    new DataItem(new FileInfo(design.Filename), "xxx")
				);
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

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}
	}
}
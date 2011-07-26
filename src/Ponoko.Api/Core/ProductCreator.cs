using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class ProductCreator : Domain {
		private readonly ProductValidator _validator;

		public ProductCreator(TheInternet internet, String baseUrl, ProductValidator validator) : base(internet, baseUrl) {
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
			var parameters = new List<Parameter> {
				new Parameter{Name = "name"	, Value = seed.Name}, 
				new Parameter{Name = "notes", Value = seed.Notes}, 
				new Parameter{Name = "ref"	, Value = seed.Reference}
			};
			
			var theFiles = new List<DataItem>();

			foreach (var design in designs) {
				parameters.AddRange(ToParameters(design));
				
				theFiles.Add(new DataItem(
				    "designs[][uploaded_data]", 
				    new FileInfo(design.Filename), "xxx"
				));
			}

			return new Payload(parameters, theFiles);
		}

		private IEnumerable<Parameter> ToParameters(Design design) {
			return new List<Parameter> {
             	new Parameter {Name = "designs[][ref]",				Value = design.Reference},
             	new Parameter {Name = "designs[][filename]",		Value = Path.GetFileName(design.Filename)},
             	new Parameter {Name = "designs[][quantity]",		Value = design.Quantity.ToString()},
             	new Parameter {Name = "designs[][material_key]",	Value = design.MaterialKey}
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

		private Error TryDeserialize(String json) {
			try {
				return ErrorDeserializer.Deserialize(json);
			} catch (Exception e) {
				throw new Exception(String.Format(
					"There was a problem deserializing the error message. The body of the response is: {0}", json), 
					e
				);
			}
		}

		private Product Deserialize(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
			return ProductDeserializer.Deserialize(json);
		}

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, payload); }
	}
}
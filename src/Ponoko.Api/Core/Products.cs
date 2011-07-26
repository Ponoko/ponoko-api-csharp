﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core {
	public class Products : Domain {
		private readonly ProductValidator _validator;

		public Products(TheInternet internet, String baseUrl, ProductValidator validator) : base(internet, baseUrl) {
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

		public void Delete(string id) {
			var uri = Map("/products/delete/{0}", id);

			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw new Exception("Delete failed");
				});

				Verify(response);
			}
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
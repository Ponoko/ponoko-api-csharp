﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class Products : Domain {
		public Products(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public void Save(Product product) {
			var theFirstDesign = product.Designs[0];

			var parameters = new NameValueCollection {
				{"name"						, product.Name}, 
				{"designs[][ref]"			, theFirstDesign.Reference},
				{"designs[][filename]"		, theFirstDesign.Filename},
				{"designs[][quantity]"		, theFirstDesign.Quantity.ToString()},
				{"designs[][material_key]"	, theFirstDesign.MaterialKey},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(theFirstDesign.Filename), "xxx"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile))) {
				if (response.StatusCode != HttpStatusCode.OK)
					throw Error(response);
			}
		}

		private Response Post(Uri uri, Payload payload) {
			return _internet.Post(uri, payload);
		}

		private Exception Error(Response response) {
			var theError = new Deserializer().Deserialize(ReadAll(response))["error"].Value<String>("message");

			return new Exception(String.Format(
				"Failed to save product. The server returned status {0} ({1}), and error message: \"{2}\"", 
				response.StatusCode, 
				(Int32)response.StatusCode, 
				theError
			));
		}
	}
}

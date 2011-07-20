﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class Products : Domain {
		public Products(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public Product Save(String name, Design design) {
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

		private Product Deserialize(Response response) {
			var payload = new Deserializer().Deserialize(ReadAll(response));

			var settings = new JsonSerializerSettings {
          		MissingMemberHandling = MissingMemberHandling.Ignore,
          		Converters = new List<JsonConverter> { new DateTimeReader() }
			};

			return JsonConvert.DeserializeObject<Product>(payload["product"].ToString(), settings);
		}

		private Response Post(Uri uri, Payload payload) { return _internet.Post(uri, payload); }

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
﻿using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class ProductFinder : Domain {
		public ProductFinder(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Product[] FindAll() {
			var json = Get(Map("/products"));

			var theList = new Deserializer().Deserialize(json);

			var result = new List<Product>();

			foreach (var productJson in theList["products"].Children()) {
				result.Add(Deserialize(productJson));
			}

			return result.ToArray();
		}

		public Product Find(String id) {
			var response = _internet.Get(Map("/products/{0}", id));

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];

			return Deserialize(productJson);
		}

		private Product Deserialize(JToken productJson) {
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}

using System;
using System.Collections.Generic;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class ProductFinder : Domain {
		public ProductFinder(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public Product[] FindAll() {
			var json = Get(Map("/products"));

			Console.WriteLine(json);

			var theList = new Deserializer().Deserialize(json);
			var result = new List<Product>();

			foreach (var productJson in theList["products"].Children()) {
				result.Add(ProductDeserializer.Deserialize(productJson.ToString()));
			}

			return result.ToArray();
		}
	}
}

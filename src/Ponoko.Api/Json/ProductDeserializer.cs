using System;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Json {
	public static class ProductDeserializer {
		public static Product Deserialize(String json)
		{
		    return SimpleDeserializer<Product>.Deserialize(json);
		}
	}
}
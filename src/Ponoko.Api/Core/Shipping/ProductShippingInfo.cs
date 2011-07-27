using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Shipping {
	public class ProductShippingInfo {
		public Int32 Quantity;

		[JsonProperty(PropertyName = "product_key")]
		public String Key;
	}
}
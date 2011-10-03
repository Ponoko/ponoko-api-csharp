using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Product {
	public class Hardware {
		public String Sku;
		public String Name;
		public Int32 Quantity;
		public String Weight;
		public String Currency;
		public Decimal Cost;
		[JsonProperty(PropertyName = "total_cost")]
		public Decimal TotalCost;
		public String Url;
	}
}
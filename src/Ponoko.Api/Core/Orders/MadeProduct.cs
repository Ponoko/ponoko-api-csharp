using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Orders {
	public class MadeProduct {
		public Int32 Quantity;
		[JsonProperty(PropertyName = "ref")]
		public String Reference;
		public String Key;
	}
}
using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Product {
	public class Urls {
		[JsonProperty(PropertyName = "make")]
		public Uri Make;
		[JsonProperty(PropertyName = "view")]
		public Uri View;
	}
}
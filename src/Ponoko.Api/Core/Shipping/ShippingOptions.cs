using System;
using System.Collections.Generic;

namespace Ponoko.Api.Core.Shipping {
	public class ShippingOptions {
		public String Currency;
		public List<Option> Options;
		public List<ProductShippingInfo> Products;
	}
}
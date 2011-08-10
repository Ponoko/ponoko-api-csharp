using System;

namespace Ponoko.Api.Core.Orders {
	public class ShippingCost {
		public Decimal Making { get; set; }
		public Decimal Shipping { get; set; }
		public Decimal Materials { get; set; }
	}
}
using System;

namespace Ponoko.Api.Core {
	public class MakeCost {
		public Decimal Total { get; set; }
		public Decimal Making { get; set; }
		public Decimal Materials { get; set; }
		public String Currency { get; set; }
	}
}
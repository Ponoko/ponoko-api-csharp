using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Orders {
	public class Order {
		[JsonProperty(PropertyName = "created_at")]
		public DateTime CreatedAt;
		public ShippingCost Cost;
		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt;
		public List<Event> Events;
		[JsonProperty(PropertyName = "tracking_numbers")]
		public List<TrackingNumber> TrackingNumbers;
		public List<MadeProduct> Products;
		[JsonProperty(PropertyName = "node_key")]
		public String NodeKey;
		[JsonProperty(PropertyName = "shipping_option_code")]
		public String ShippingOptionCode;
		[JsonProperty(PropertyName = "ref")]
		public String Reference;
		public String Key;
		[JsonProperty(PropertyName = "shipped")]
		public Boolean HasShipped;
	}

	public class Event {
		[JsonProperty(PropertyName = "completed_at")]
		public DateTime CompletedAt;
		public String Name;
	}

	public class TrackingNumber {
		public String Value;
	}

	public class ShippingCost {
		public Decimal Making { get; set; }
		public Decimal Shipping { get; set; }
		public Decimal Materials { get; set; }
	}

	public class MadeProduct {
		public Int32 Quantity;
		[JsonProperty(PropertyName = "ref")]
		public String Reference;
		public String Key;
	}
}

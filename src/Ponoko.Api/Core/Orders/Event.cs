using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Orders {
	public class Event {
		[JsonProperty(PropertyName = "completed_at")]
		public DateTime CompletedAt;
		public String Name;
	}
}
using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class ErrorMessage {
		public String Type { get; set; }

		[JsonProperty(PropertyName = "node_key")]
		public String NodeKey { get; set; }
		
		[JsonProperty(PropertyName = "error_code")]
		public String ErrorCode { get; set; }
		
		[JsonProperty(PropertyName = "error_message")]
		public String Value { get; set; }

		public String Name { get; set; }

		[JsonProperty(PropertyName = "material_key")]
		public String MaterialKey { get; set; }
	}
}
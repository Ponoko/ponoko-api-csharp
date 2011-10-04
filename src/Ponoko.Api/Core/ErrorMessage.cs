using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class ErrorMessage {
		public String Type;

		[JsonProperty(PropertyName = "node_key")] 
		public String NodeKey;

		[JsonProperty(PropertyName = "error_code")] 
		public String ErrorCode;

		[JsonProperty(PropertyName = "error_message")] 
		public String Value;

		public String Name;

		[JsonProperty(PropertyName = "material_key")] 
		public String MaterialKey;
	}
}
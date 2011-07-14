using System;
using Newtonsoft.Json.Linq;

namespace Ponoko.Api.Json {
	public class Deserializer {
		public JObject Deserialize(String json) { return JObject.Parse(json); }
	}
}

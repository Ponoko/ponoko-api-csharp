using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Node {
		[JsonProperty(PropertyName = "materials_updated_at")]
		public DateTime MaterialsUpdatedAt;
		public String Name;
		public String Key;
	}
}
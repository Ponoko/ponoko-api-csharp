using System;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Material {
		public readonly NameValueCollection Attributes = new NameValueCollection();
		public String Key;
		public String Name;
		public String Type;

		[JsonProperty(PropertyName = "updated_at")] 
		public DateTime UpdatedAt;
	}
}

using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Design {
		public Int64 Size{ get; set; }

		[JsonProperty(PropertyName = "created_at")]
		public DateTime CreatedAt { get; set; }

		public Int32 Quantity { get; set; }

		[JsonProperty(PropertyName = "content_type")]
		public String ContentType { get; set; }

		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty(PropertyName = "material_key")]
		public String MaterialKey { get; set; }

		public String Filename { get; set; }

		[JsonProperty(PropertyName = "ref")]
		public String Reference { get; set; }

		public String Key { get; set; }

		[JsonProperty(PropertyName = "make_cost")]
		public MakeCost MakeCost { get; set; }
	}
}
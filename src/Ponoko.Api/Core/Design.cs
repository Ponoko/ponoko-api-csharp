using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Design {
		public Int64 Size;

		[JsonProperty(PropertyName = "created_at")] 
		public DateTime CreatedAt;

		public Int32 Quantity { get; set; }

		[JsonProperty(PropertyName = "content_type")] 
		public String ContentType;

		[JsonProperty(PropertyName = "updated_at")] 
		public DateTime UpdatedAt;

		[JsonProperty(PropertyName = "material_key")] 
		public String MaterialKey;

		public String Filename;

		[JsonProperty(PropertyName = "ref")] 
		public String Reference;

		public String Key;

		[JsonProperty(PropertyName = "make_cost")] 
		public MakeCost MakeCost;
	}
}
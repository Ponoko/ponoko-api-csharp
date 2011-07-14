using System;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Material {
		public String Color;
		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt { get; set; }
		public String Weight;
		public String Type;
		public String Key;
		public String Thickness;
		public String Name;
		public String Width;
		[JsonProperty(PropertyName = "material_type")]
		public String MaterialType;
		public String Length;
		public String Kind;
	}
}

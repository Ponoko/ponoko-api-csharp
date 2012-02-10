using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Product {
	public class Product {
		public readonly List<Design> Designs = new List<Design>();
		
		[JsonProperty(PropertyName = "design_images")]
		public readonly List<File> DesignImages = new List<File>();
		
		[JsonProperty(PropertyName = "assembly_instructions")]
		public readonly List<File> AssemblyInstructions = new List<File>();
		
		public readonly List<Hardware> Hardware = new List<Hardware>();

		[JsonProperty(PropertyName = "key")] 
		public String Key;
		
		[JsonProperty(PropertyName = "ref")] 
		public String Reference;
		
		[JsonProperty(PropertyName = "node_key")] 
		public String NodeKey;
		public String Name;
		public String Description;
		public String Notes;

		[JsonProperty(PropertyName = "created_at")] 
		public DateTime CreatedAt;

		[JsonProperty(PropertyName = "updated_at")] 
		public DateTime UpdatedAt;

		[JsonProperty(PropertyName = "locked?")] 
		public Boolean IsLocked;
		
		[JsonProperty(PropertyName = "materials_available?")] 
		public Boolean AreMaterialsAvailable;

		[JsonProperty(PropertyName = "total_make_cost")] 
		public MakeCost TotalMakeCost;

		[JsonProperty(PropertyName = "urls")]
		public Urls Urls;
	}
}

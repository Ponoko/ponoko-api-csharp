using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ponoko.Api.Core.Product {
	public class Product {
		private readonly List<Design> _designs = new List<Design>();
		private readonly List<File> _designImages = new List<File>();
		private	readonly List<File> _assemblyInstructions = new List<File>();
		private	readonly List<Hardware> _hardware = new List<Hardware>();

		[JsonProperty(PropertyName = "key")]
		public String Key { get; set; }

		[JsonProperty(PropertyName = "ref")]
		public String Reference { get; set; }

		[JsonProperty(PropertyName = "node_key")]
		public String NodeKey { get; set; }

		public String Name { get; set; }
		public String Description { get; set; }
		public String Notes{ get; set; }

		[JsonProperty(PropertyName = "created_at")]
		public DateTime CreatedAt { get; set; }
		
		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt { get; set; }

		public List<Design> Designs { get { return _designs; } }
		
		[JsonProperty(PropertyName = "locked?")]
		public Boolean IsLocked { get; set; }
		
		[JsonProperty(PropertyName = "materials_available?")]
		public Boolean AreMaterialsAvailable { get; set; }

		[JsonProperty(PropertyName = "design_images")]
		public List<File> DesignImages { get { return _designImages; } }

		[JsonProperty(PropertyName = "assembly_instructions")]
		public List<File> AssemblyInstructions { get { return _assemblyInstructions; } }

		public List<Hardware> Hardware { get { return _hardware; } }

		[JsonProperty(PropertyName = "total_make_cost")]
		public MakeCost TotalMakeCost { get; set; }
	}
}

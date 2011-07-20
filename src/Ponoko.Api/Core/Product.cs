﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ponoko.Api.Core {
	public class Product {
		[JsonProperty(PropertyName = "key")]
		public String Key { get; set; }

		[JsonProperty(PropertyName = "ref")]
		public String Reference { get; set; }

		[JsonProperty(PropertyName = "node_key")]
		public String NodeKey { get; set; }

		public String Name { get; set; }
		public String Notes{ get; set; }

		[JsonProperty(PropertyName = "created_at")]
		public DateTime CreatedAt { get; set; }
		
		[JsonProperty(PropertyName = "updated_at")]
		public DateTime UpdatedAt { get; set; }

		private readonly List<Design> _designs = new List<Design>();
		public List<Design> Designs { get { return _designs; } }
		
		[JsonProperty(PropertyName = "is_locked")]
		public Boolean IsLocked { get; set; }
		
		[JsonProperty(PropertyName = "is_materials_available")]
		public Boolean AreMaterialsAvailable { get; set; }
	}

	public class Design {
		public String Reference { get; set; }
		public String Filename { get; set; }
		public Int32 Quantity { get; set; }
		public String MaterialKey { get; set; }
	}
}
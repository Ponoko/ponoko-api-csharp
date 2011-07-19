using System;
using System.Collections.Generic;

namespace Ponoko.Api.Core {
	public class Product {
		private readonly List<Design> _designs = new List<Design>();
		
		public List<Design> Designs {
			get { return _designs; }
		}

		public String Name{ get; set; }	
	}

	public class Design {
		public String Reference { get; set; }
		public String Filename { get; set; }
		public Int32 Quantity { get; set; }
		public String MaterialKey { get; set; }
	}
}

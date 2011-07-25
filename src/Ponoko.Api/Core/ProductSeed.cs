using System;

namespace Ponoko.Api.Core {
	public class ProductSeed {
		public static ProductSeed WithName(String name) {
			return new ProductSeed {Name = name};
		}

		public String Name;
		public String Notes;
		public String Reference;
	}
}
using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public static class ExampleMaterials {
		public const String METAL = "6b0fa5b03269012e3438404062cdb04a";
		public const String ACRYLIC = "68876ab03269012e2fd9404062cdb04a";
		public const String DURABLE_PLASTIC = "6bb50fd03269012e3526404062cdb04a";
		public const String SUPERFINE_PLASTIC = "6bb5ac203269012e3528404062cdb04a";	
	}

	public class ProductAcceptanceTest : AcceptanceTest {
		protected CreateCommand CreateCommand { get; set; }

		[TestFixtureSetUp]
		public void BeforeAll() {
			CreateCommand = new CreateCommand(Internet, Settings.BaseUrl);
		}

		protected Product NewProduct(String called) {
			return CreateCommand.Create(ProductSeed.WithName(called), NewDesign());
		}

		protected void Delete(Product product) {
			un.less(null == product, () => new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key));
		}

		protected Design NewDesign() {
			const String VALID_MATERIAL_KEY = "6bb50fd03269012e3526404062cdb04a";

			return new Design {
              	Filename	= new FileInfo(@"res\bottom_new.stl").FullName,
              	MaterialKey = VALID_MATERIAL_KEY,
              	Quantity	= 1,
              	Reference	= "42"
			};
		}
	}
}
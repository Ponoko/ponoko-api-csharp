using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Json;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public static class ExampleMaterials {
		public const String METAL = "6b0fa5b03269012e3438404062cdb04a";
		public const String ACRYLIC = "68876ab03269012e2fd9404062cdb04a";
		public const String DURABLE_PLASTIC = "6bb50fd03269012e3526404062cdb04a";
		public const String SUPERFINE_PLASTIC = "6bb5ac203269012e3528404062cdb04a";	
	}

	public class ProductAcceptanceTest : AcceptanceTest {
		public CreateCommand CreateCommand { get; set; }

		[SetUp]
		public void BeforeEach() {
			CreateCommand = new CreateCommand(Internet, Settings.BaseUrl, new DefaultProductValidator());
		}

		protected void given_at_least_one_product() {
			given_at_least_one_product("example");
		}

		protected void given_at_least_one_product(String called) {
			NewProduct(called);
		}

		protected Product NewProduct(String called) {
			return CreateCommand.Create(ProductSeed.WithName(called), NewDesign());
		}

		protected String FindFirstProductKey() {
			var uri = Map("/products");

			using (var response = Get(uri)) {
				var temp = new Deserializer().Deserialize(Body(response));
				
				var products = temp["products"];

				Assert.That(products.HasValues, "There are zero products, so unable to return the first one.");
				
				return products.First.Value<String>("key");
			}
		}

		protected void Delete(Product product) {
			Assert.Throws<Exception>(() => 
				new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key), 
			    "Expected this to fail because at the time of writing the delete operation is broken on the server end. " + 
			    "It didn't fail, so the remote end has been fixed and you can remove this assertion."
			);
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
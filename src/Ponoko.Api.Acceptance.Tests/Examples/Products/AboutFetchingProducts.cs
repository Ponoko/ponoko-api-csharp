using System;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutFetchingProducts : ProductAcceptanceTest {
		private Product AnyProduct;

		[TestFixtureSetUp]
		public void TestFixtureSetUp() {
			AnyProduct = NewProduct("At least one for testing find");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() { Delete(AnyProduct); }

		[Test]
		public void you_can_get_a_list_of_products() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var result = finder.FindAll();

			Assert.That(result.Length, Is.GreaterThan(0), "Expected at least one product returned");
		}

		[Test]
		public void you_can_get_a_single_product() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var result = finder.Find(AnyProduct.Key);

			Assert.IsNotNull(result, "Expected a non-null result");
			Assert.AreEqual(result.Name, AnyProduct.Name, "Unexpected name");
			Assert.AreEqual("42", result.Designs[0].Reference, "Unexpected name");
		}

		[Test]
		public void you_can_check_existence_of_a_product() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			
			var result = finder.Exists(AnyProduct.Key);
			Assert.IsTrue(result, "Expected the result to be true because the Product does exist.");
			
			const String AN_ID_THAT_DOES_NOT_EXIST = "Phil Murphy's fanny pack";
			result = finder.Exists(AN_ID_THAT_DOES_NOT_EXIST);

			Assert.IsFalse(result, "Expected the result to be false because the Product does not exist.");
		}

		[Test] 
		public void finding_a_product_that_does_not_exist_returns_null() {
			const String AN_ID_THAT_DOES_NOT_EXIST = "Phil Murphy's fanny pack";

			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var result = finder.Find(AN_ID_THAT_DOES_NOT_EXIST);

			Assert.IsNull(result, "Expected finding a product that does not exist to return null.");	
		}

		[Test, Ignore("PENDING")]
		public void product_may_include_an_auto_generated_design_image() {}
	}
}

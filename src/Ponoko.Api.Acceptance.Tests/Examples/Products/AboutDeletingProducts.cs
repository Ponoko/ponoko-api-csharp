using System;
using NUnit.Framework;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDeletingProducts : ProductAcceptanceTest {
		[Test]
		public void can_delete_a_product() {
			var product = NewProduct("A product for testing deletes");

			new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key);

			then_the_product_does_not_exist_with_key(product.Key);
		}

		private void then_the_product_does_not_exist_with_key(String id) {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var result = finder.Find(id);

			Assert.IsNull(result, "Expected that finding the product with key <{0}> would return null.", id);
		}

		// [Test] public void it_deletes_all_products_with_the_same_key() { }
		// [Test] public void cannot_delete_a_product_that_has_been_made() { }
	}
}

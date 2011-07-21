using System;
using NUnit.Framework;
using Ponoko.Api.Core;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDeletingroducts : ProductAcceptanceTest {
		[Test]
		public void can_delete_a_product() {
			given_at_least_one_product();
			
			var id = FindFirstProductKey();

			Products.Delete(id);

			then_the_product_does_not_exist_with_key(id);
		}

		private void then_the_product_does_not_exist_with_key(String id) {
			var finder = new ProductFinder(Internet, Settings.BaseUrl);
			var result = finder.Find(id);

			Assert.IsNull(result, "Expected that finding the product with key <{0}> would return null.", id);
		}

		// [Test] public void can_delete_all_products() { }
		// [Test] public void it_deletes_all_products_with_the_same_key() { }
	}
}

using System;
using NUnit.Framework;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDeletingProducts : ProductAcceptanceTest {
		[Test]
		public void you_can_delete_a_product() {
			var product = NewProduct("A product for testing deletes");

			new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key);

			then_the_product_does_not_exist_with_key(product.Key);
		}
		
		[Test]
		public void you_can_delete_them_all() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var delete = new DeleteCommand(Internet, Settings.BaseUrl);
			var result = finder.FindAll();

			var count = 0;

			Console.WriteLine("Deleting {0} products", result.Length);

			foreach (var product in result) {
				delete.Delete(product.Key);
				count++;
			}

			Console.WriteLine("Deleted {0} products", count);

			Assert.AreEqual(0, finder.FindAll().Length, "Expected them all to have been deleted");
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

using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutFetchingProducts : ProductAcceptanceTest {
		[Test]
		public void can_get_a_list_of_products() {
			given_at_least_one_product();

			var finder = new ProductFinder(Internet, Settings.BaseUrl);
			var result = finder.FindAll();

			Assert.That(result.Length, Is.GreaterThan(0), "Expected at least one product returned");
		}

		[Test]
		public void can_get_a_single_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			
			var finder = new ProductFinder(Internet, Settings.BaseUrl);
			var result = finder.Find(id);

			Assert.IsNotNull(result, "Expected a non-null result");
			Assert.AreEqual(result.Name, "example", "Unexpected name");
			Assert.AreEqual(result.Designs[0].Reference, "1337", "Unexpected name");
		}

		[Test]
		public void you_can_check_existence_of_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			var uri = Map("/products/{0}", id);

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			}

			uri = Map("/products/{0}", "MUST_NOT_EXIST");

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
			}
		}
	}
}

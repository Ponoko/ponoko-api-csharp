using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutFetchingProducts : ProductAcceptanceTest {
		[Test]
		public void can_get_a_list_of_products() {
			given_at_least_one_product();

			var uri = Map("/products");

			using (var response = Get(uri)) {
				var result = new Deserializer().Deserialize(Body(response));
				
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");

				var products = result["products"].First.Value<String>("key");

				Console.WriteLine(products);
			}
		}

		[Test]
		public void can_get_a_single_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			var uri = Map("/products/{0}", id);

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Console.WriteLine(Json(response));
			}
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

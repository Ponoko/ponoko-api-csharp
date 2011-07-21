using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDeletingroducts : ProductAcceptanceTest {
		[Test]
		public void can_delete_a_product() {
			given_at_least_one_product();
			
			var id = FindFirstProductKey();
			var uri = Map("/products/delete/{0}", id);

			using (var response = Get(Map("/products/{0}", id))) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
					"Expected the product to exist before we delete it."
				);
			}

			var theStatusReturnedByDelete = -1;

			using (var response = Post(uri, Payload.Empty)) {
				theStatusReturnedByDelete = (Int32)response.StatusCode;
			}

			using (var response = Get(Map("/products/{0}", id))) {
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
					"Expected the product to have been deleted, but it's still there."
				);
			}

			Assert.AreEqual(200, theStatusReturnedByDelete, "Expected delete to return status 200.");
		}

		// [Test] public void can_delete_all_products() { }
	}
}

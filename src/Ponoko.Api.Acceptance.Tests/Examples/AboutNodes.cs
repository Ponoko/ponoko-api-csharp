using System.Net;
using NUnit.Framework;

namespace Ponoko.Api.Acceptance.Tests.Examples {
    [TestFixture]
    public class AboutNodes : AcceptanceTest {
    	[Test]
        public void can_get_nodes_which_represent_the_available_making_nodes() {
            var uri = Map("/nodes");
			
			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
				
				var json = Json(response);

				Assert.That(json, Is.StringMatching("\"name\": \"Ponoko - United States\""));
			}
        }

		// TEST: It rejects an invalid consumer key when everything else is okay
		// TEST: using the same nonce and timestamp combination results in 401
    }
}

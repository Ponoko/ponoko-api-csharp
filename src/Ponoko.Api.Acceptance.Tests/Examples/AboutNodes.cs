﻿using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Rest;

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

		[Test]
        public void can_get_options() {
            var uri = Map("/nodes");
			
			using (var response = Options(uri, Payload.Empty)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
				
				var json = Json(response);

				Console.WriteLine(json);

				Assert.That(json, Is.StringMatching("\"name\": \"Ponoko - United States\""));
			}
        }

		// TEST: It rejects an invalid consumer key when everything else is okay
		// TEST: create product fails without a file
		// TEST: you must supply a valid material when creating product
		// TEST: using the same nonce and timestamp combination results in 401
    }
}

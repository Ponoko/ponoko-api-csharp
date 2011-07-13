using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Net.Json;

namespace Ponoko.Net.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutMaterials : AcceptanceTest {
		[Test]
		public void can_get_materials_catalogue() {
			var nodeKey = "2e9d8c90326e012e359f404062cdb04a";
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
				Console.WriteLine(Json(response));
			}
		}

		[Test]
		public void and_you_can_deserialize_the_result() {
			var nodeKey = "2e9d8c90326e012e359f404062cdb04a";
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
				var json = Json(response);
				var result = MaterialListDeserializer.Deserialize(json);
				Assert.AreEqual(347, result.Length);
			}
		}
	}
}

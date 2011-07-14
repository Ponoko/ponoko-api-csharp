using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutMaterials : AcceptanceTest {
		private String _firstNodeKey;

		[Test]
		public void first_you_need_key_for_a_making_node() {
			var uri = Map("/nodes");
			
			using (var response = Get(uri, Credentials)) {
				var json = new Deserializer().Deserialize(Body(response));

				var theNodeKey = json["nodes"].First.Value<String>("key");
				
				Assert.IsNotEmpty(theNodeKey, "Expected at least one node key.");
			}
		}

		[Test]
		public void can_get_materials_catalogue() {
			var nodeKey = FirstNodeKey;
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
			}
		}

		[Test]
		public void and_you_can_deserialize_the_result() {
			var nodeKey = FirstNodeKey;
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");
				var json = Json(response);
				var result = MaterialListDeserializer.Deserialize(json);
				Assert.AreEqual(347, result.Length);
			}
		}

		private String FirstNodeKey {
			get { return _firstNodeKey ?? (_firstNodeKey = FindFirstNodeKey()); }
		}

		private string FindFirstNodeKey() {
			var uri = Map("/nodes");

			using (var response = Get(uri, Credentials)) {
				var json = new Deserializer().Deserialize(Body(response));

				return json["nodes"].First.Value<String>("key");
			}
		}
	}
}

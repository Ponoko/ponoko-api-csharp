using System;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Json;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class NodeDeserializerTests {
		[Test] 
		public void you_can_deserialize_a_single_node() {
			var json = 
				"{" +
				"	\"materials_updated_at\": \"0064/06/19 07:00:00 +0000\"," +
				"	\"name\": \"Ponoko - United States\"," +
				"	\"key\": \"2e9d8c90326e012e359f133762cdb04a\"" +
				"}";

			var result = SimpleDeserializer<Node>.Deserialize(json);

			var greatFireOfRomeBegins = new DateTime(0064, 6, 19, 7, 0, 0, DateTimeKind.Utc);

			Assert.AreEqual(greatFireOfRomeBegins, result.MaterialsUpdatedAt);
			Assert.AreEqual("Ponoko - United States", result.Name);
			Assert.AreEqual("2e9d8c90326e012e359f133762cdb04a", result.Key);
		}

		[Test]
		public void or_an_array_of_nodes() {
			var json = "{ \"nodes\" : [" + 
				"{" +
				"	\"materials_updated_at\": \"0064/06/19 07:00:00 +0000\"," +
				"	\"name\": \"Ponoko - United States\"," +
				"	\"key\": \"key_0\"" +
				"}," + 
				"{" +
				"	\"materials_updated_at\": \"0064/06/19 07:00:00 +0000\"," +
				"	\"name\": \"Ponoko - United States\"," +
				"	\"key\": \"key_1\"" +
				"}" +
			"]}";

			var result = NodeListDeserializer.Deserialize(json);

			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("key_0", result[0].Key);
			Assert.AreEqual("key_1", result[1].Key);
		}
	}
}

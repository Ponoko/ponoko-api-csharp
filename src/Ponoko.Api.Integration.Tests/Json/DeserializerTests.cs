using System;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class DeserializerTests {
		[Test] public void 
		for_example_you_can_deserialize_to_a_hash() {
			var json = "{" +
			"	'nodes': [" +
			"		{" +
			"			'materials_updated_at': '2011/06/27 20:23:16 +0000'," +
			"			'key': '2e9d8c90326e012e359f404062cdb04a'," +
			"			'name': 'Ponoko - United States'" + 
			"		}" +
 			"	]" +
			"}";
			
			var result = new Deserializer().Deserialize(json);

			Assert.AreEqual("Ponoko - United States", result["nodes"].First.Value<String>("name"));
		}
	}
}

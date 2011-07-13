using System;
using NUnit.Framework;
using Ponoko.Net.Json;

namespace Ponoko.Net.Unit.Tests.Json {
	class MaterialDeserializerTests {
		[Test] public void 
		you_can_deserialize_a_single_material() {
			var json = "{" + 
				"\"updated_at\": \"2011/03/17 02:08:51 +0000\"," +
				"\"type\": \"P1\"," +
				"\"weight\": \"0.1 kg\"," +
				"\"color\": \"Fuchsia\"," +
				"\"key\": \"6812d5403269012e2f2f404062cdb04a\"," +
				"\"thickness\": \"3.0 mm\"," +
				"\"name\": \"Felt\"," +
				"\"width\": \"181.0 mm\"," +
				"\"material_type\": \"sheet\"," +
				"\"length\": \"181.0 mm\"," +
				"\"kind\": \"Fabric\"" +
			"}";

			var result = MaterialDeserializer.Deserialize(json);

			var expectedDate = new DateTime(2011, 3, 17, 2, 8, 51, DateTimeKind.Utc);

			Assert.AreEqual(expectedDate						, result.UpdatedAt);
			Assert.AreEqual("P1"								, result.Type);
			Assert.AreEqual("0.1 kg"							, result.Weight);
			Assert.AreEqual("Fuchsia"							, result.Color);
			Assert.AreEqual("6812d5403269012e2f2f404062cdb04a"	, result.Key);
			Assert.AreEqual("3.0 mm"							, result.Thickness);
			Assert.AreEqual("Felt"								, result.Name);
			Assert.AreEqual("181.0 mm"							, result.Width);
			Assert.AreEqual("sheet"								, result.MaterialType);
			Assert.AreEqual("181.0 mm"							, result.Length);
			Assert.AreEqual("Fabric"							, result.Kind);
		}

		[Test] public void 
		or_an_array_of_materials() {
			var json = "{\"materials\": [{" + 
				"\"updated_at\": \"2011/03/17 02:08:51 +0000\"," +
				"\"type\": \"P1\"," +
				"\"weight\": \"0.1 kg\"," +
				"\"color\": \"Fuchsia\"," +
				"\"key\": \"key_0\"," +
				"\"thickness\": \"3.0 mm\"," +
				"\"name\": \"Felt\"," +
				"\"width\": \"181.0 mm\"," +
				"\"material_type\": \"sheet\"," +
				"\"length\": \"181.0 mm\"," +
				"\"kind\": \"Fabric\"" +
				"}," + 
				"{" + 
				"\"updated_at\": \"2011/03/17 02:08:51 +0000\"," +
				"\"type\": \"P1\"," +
				"\"weight\": \"0.1 kg\"," +
				"\"color\": \"Black\"," +
				"\"key\": \"key_1\"," +
				"\"thickness\": \"7.0 mm\"," +
				"\"name\": \"Brick\"," +
				"\"width\": \"181.0 mm\"," +
				"\"material_type\": \"block\"," +
				"\"length\": \"181.0 mm\"," +
				"\"kind\": \"Stone\"" +
				"}" + 
			"]}";

			var result = MaterialListDeserializer.Deserialize(json);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("key_0", result[0].Key);
			Assert.AreEqual("key_1", result[1].Key);
		}
	}
}

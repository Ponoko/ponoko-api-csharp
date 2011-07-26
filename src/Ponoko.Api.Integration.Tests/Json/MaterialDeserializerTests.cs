using System;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	class MaterialDeserializerTests {
		[Test] 
		public void you_can_deserialize_a_single_material() {
			var json = "{" + 
				"	'updated_at': '2011/03/17 02:08:51 +0000'," +
				"	'type': 'P1'," +
				"	'weight': '0.1 kg'," +
				"	'color': 'Fuchsia'," +
				"	'key': '6812d5403269012e2f2f404062cdb04a'," +
				"	'thickness': '3.0 mm'," +
				"	'name': 'Felt'," +
				"	'width': '181.0 mm'," +
				"	'material_type': 'sheet'," +
				"	'length': '181.0 mm'," +
				"	'kind': 'Fabric'" +
				"}";

			var result = MaterialDeserializer.Deserialize(json);

			var expectedDate = new DateTime(2011, 3, 17, 2, 8, 51, DateTimeKind.Utc);

			Assert.AreEqual("6812d5403269012e2f2f404062cdb04a"	, result.Key);
			Assert.AreEqual("Felt"								, result.Name);
			Assert.AreEqual("P1"								, result.Type);
			Assert.AreEqual(expectedDate						, result.UpdatedAt);
		}

		[Test] 
		public void or_an_entire_catalogue_of_materials() {
			var json = "{" +
				"'materials': [" + 
				"	{" + 
				"		'updated_at': '2011/03/17 02:08:51 +0000'," +
				"		'type': 'P1'," +
				"		'weight': '0.1 kg'," +
				"		'color': 'Fuchsia'," +
				"		'key': 'key_0'," +
				"		'thickness': '3.0 mm'," +
				"		'name': 'Felt'," +
				"		'width': '181.0 mm'," +
				"		'material_type': 'sheet'," +
				"		'length': '181.0 mm'," +
				"		'kind': 'Fabric'" +
				"	}," + 
				"	{" + 
				"		'updated_at': '2011/03/17 02:08:51 +0000'," +
				"		'type': 'P1'," +
				"		'weight': '0.1 kg'," +
				"		'color': 'Black'," +
				"		'key': 'key_1'," +
				"		'thickness': '7.0 mm'," +
				"		'name': 'Brick'," +
				"		'width': '181.0 mm'," +
				"		'material_type': 'block'," +
				"		'length': '181.0 mm'," +
				"		'kind': 'Stone'" +
				"	}" + 
				"]}";

			var result = MaterialCatalogueDeserializer.Deserialize(json);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("key_0", result[0].Key);
			Assert.AreEqual("key_1", result[1].Key);
		}

		[Test]
		public void type_specific_data_varies_between_materials_and_is_available_as_a_lookup() {
			var json = "{" + 
				"	'updated_at': '2011/03/17 02:08:51 +0000'," +
				"	'type': 'P1'," +
				"	'weight': '0.1 kg'," +
				"	'color': 'Fuchsia'," +
				"	'key': '6812d5403269012e2f2f404062cdb04a'," +
				"	'thickness': '3.0 mm'," +
				"	'name': 'Felt'," +
				"	'width': '181.0 mm'," +
                "	'nick_name': \"Graeme's face\","+                
				"	'material_type': 'sheet'," +
				"	'length': '181.0 mm'," +
				"	'kind': 'Fabric'" +
				"}";

			var result = MaterialDeserializer.Deserialize(json);
			
			Assert.AreEqual(8, result.Attributes.Count, 
				"Expected that only those attributes which do not correspond to " + 
				"Material properties to be included in the Attributes collection."
			);

			Assert.AreEqual("0.1 kg"		, result.Attributes["weight"]);
			Assert.AreEqual("Fuchsia"		, result.Attributes["color"]);
			Assert.AreEqual("3.0 mm"		, result.Attributes["thickness"]);
			Assert.AreEqual("181.0 mm"		, result.Attributes["width"]);
			Assert.AreEqual("sheet"			, result.Attributes["material_type"]);
			Assert.AreEqual("181.0 mm"		, result.Attributes["length"]);
			Assert.AreEqual("Fabric"		, result.Attributes["kind"]);
			Assert.AreEqual("Graeme's face"	, result.Attributes["nick_name"]);
		}

		[Test]
		public void type_specific_data_excludes_attributes_that_are_present_as_material_properties() {
			var json = "{" + 
				"	'updated_at': '2011/03/17 02:08:51 +0000'," +
				"	'type': 'P1'," +
				"	'weight': '0.1 kg'," +
				"	'color': 'Fuchsia'," +
				"	'key': '6812d5403269012e2f2f404062cdb04a'," +
				"	'thickness': '3.0 mm'," +
				"	'name': 'Felt'," +
				"	'width': '181.0 mm'," +
				"	'material_type': 'sheet'," +
				"	'length': '181.0 mm'," +
				"	'kind': 'Fabric'" +
				"}";

			var result = MaterialDeserializer.Deserialize(json);

			var theIgnoredAttributes = new String[] {"key", "name", "type", "updated_at"};

			foreach (var attribute in theIgnoredAttributes) {
				Assert.IsNull(result.Attributes.Get(attribute), 
					"Expected that the attributes collection should not contain key <{0}>", attribute
				);
			}
		}
	}
}

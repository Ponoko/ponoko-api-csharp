using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class ProductDeserializerTests {
		[Test]
		public void can_deserialize_a_product() {
			var json = "{" + 
				"	'key': '1234', "  +
				"	'ref': '4321', " + 
				"	'node_key': '1234', " + 
				"	'name': 'tea pot', " + 
				"	'notes': 'has a special non-drip spout', " + 
				"	'created_at': '2011/01/01 12:00:00 +0000', " + 
				"	'updated_at': '2011/07/25 05:00:00 +0000', " + 
				"	'locked?' :'false', " +
				"	'materials_available?': 'true', " + 
				"	'designs': [" +
				"		{" + 
				"			'key': '1234', " + 
				"			'ref': '4321', " + 
				"			'created_at': '2011/01/01 12:00:00 +0000', " + 
				"			'updated_at': '2011/07/25 05:00:00 +0000', " + 
				"			'size': 9999, " + 
				"			'filename': 'teapot.eps', " + 
				"			'quantity': 1, " + 
				"			'content_type': 'application/postscript', " + 
				"			'material_key': '1234'," + 
				"			'make_cost': {'currency': 'USD', 'making': '56.78', 'materials': '56.78', 'total': '56.78'}" + 
				"		}" + 
				"	], " +
				"   'design_images': [{'filename': 'jazz_kang_is_now_swiss.stl.png'}], " +
				"   'assembly_instructions': [{'filename': 'phil_murphy_bike_shorts.pdf'}], " +
				"	'hardware': [ " +
				"		{" + 
				"			'sku': 'GPS-08254', " + 
                "			'name': 'Widget', " +
                "			'quantity': '3', " +
                "			'weight': '0.1kg'," + 
                "			'currency': 'USD', " +
                "			'cost': '9.99', " +
                "			'total_cost': '29.97'" + 
				"		}" + 
				"	], " +
				"   'total_make_cost': {'currency': 'USD', 'making': '56.78', 'materials': '56.78', 'total': '56.78', 'hardware': '0.99'}" + 
			"}";

			var result = ProductDeserializer.Deserialize(json);

			Assert.AreEqual("1234", result.Key				, "Unexpected key");
			Assert.AreEqual("4321", result.Reference		, "Unexpected reference");
			Assert.AreEqual("1234", result.NodeKey			, "Unexpected node key");
			Assert.AreEqual("tea pot", result.Name			, "Unexpected name");
			Assert.AreEqual("has a special non-drip spout"	, result.Notes, "Unexpected notes");
			
			var januaryFirst2011 = new DateTime(2011, 1, 1, 12, 0, 0);
			var fiveAMonTheColdestDayInWellingtonEver = new DateTime(2011, 7, 25, 5, 0, 0);

			Assert.AreEqual(fiveAMonTheColdestDayInWellingtonEver, result.UpdatedAt, "Unexpected \"updated at\" date");
			Assert.AreEqual(januaryFirst2011, result.CreatedAt	, "Unexpected \"created at\" date");
			Assert.IsFalse(result.IsLocked						, "Expected the product to be unlocked");
			Assert.IsTrue(result.AreMaterialsAvailable			, "Expected the product to have materials available based on the supplied json");

			Assert.AreEqual(1, result.Designs.Count, "Expected exactly one design");

			Assert.AreEqual(56.78, result.TotalMakeCost.Making, "Unexpected making cost");
			Assert.AreEqual(56.78, result.TotalMakeCost.Materials, "Unexpected materials cost");
			Assert.AreEqual(56.78, result.TotalMakeCost.Total, "Unexpected total cost");
			Assert.AreEqual(0.99, result.TotalMakeCost.Hardware, "Unexpected hardware cost");
			
			Assert.AreEqual(1, result.DesignImages.Count, "Unexpected number of design images");
			Assert.AreEqual(result.DesignImages.First().Filename, "jazz_kang_is_now_swiss.stl.png", "The first design image does not match");

			Assert.AreEqual(1, result.AssemblyInstructions.Count, "Unexpected number of assembly instructions");
			Assert.AreEqual(result.AssemblyInstructions.First().Filename, "phil_murphy_bike_shorts.pdf", "The first assembly instruction does not match");

			Assert.AreEqual(1, result.Hardware.Count, "Expected one hardware");
		}

		[Test]
		public void can_deserialize_product_earls() {
			var json = "{" +
				"	'urls': {" +
				"		'make': 'http://sandbox.ponoko.com/make/new/5330eef287c04b9af1ee84bafa6f0d78'," +
				"		'view': 'http://sandbox.ponoko.com/products/show/5330eef287c04b9af1ee84bafa6f0d78'" +
				"	}," +
				"}";

			var result = ProductDeserializer.Deserialize(json);

			Assert.AreEqual("http://sandbox.ponoko.com/make/new/5330eef287c04b9af1ee84bafa6f0d78", result.Urls.Make.ToString());
			Assert.AreEqual("http://sandbox.ponoko.com/products/show/5330eef287c04b9af1ee84bafa6f0d78", result.Urls.View.ToString());
		}
	}
}
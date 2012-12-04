using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Core;
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

		[Test]
		public void can_deserialize_a_product_that_includes_design_with_units_bounding_box_and_volume() {
			var json = @"{
			  'name': 'Example for testing assembly instructions',
			  'created_at': '2012/05/12 02:22:32 +0000',
			  'urls': {
				'make': 'http://sandbox.ponoko.com/make/new/d853602f61867bf470722824e7ab2c90',
				'view': 'http://sandbox.ponoko.com/products/show/d853602f61867bf470722824e7ab2c90'
			  },
			  'updated_at': '2012/05/12 02:22:34 +0000',
			  'locked?': false,
			  'total_make_cost': {
				'total': '18.85',
				'making': '16.02',
				'materials': '2.83',
				'currency': 'USD',
				'hardware': '0.00'
			  },
			  'node_key': '2e9d8c90326e012e359f404062cdb04a',
			  'ref': '',
			  'description': '',
			  'key': 'd853602f61867bf470722824e7ab2c90',
			  'materials_available?': true,
			  'designs': [
				{
				  'size': 137984,
				  'created_at': '2012/05/12 02:22:32 +0000',
				  'quantity': 1,
				  'content_type': 'application/stl',
				  'updated_at': '2012/05/12 02:22:38 +0000',
				  'units': 'mm',
				  'bounding_box': {
					'x': 14.0,
					'y': 100.0,
					'z': 50.0
				  },
				  'volume': 12351.5312,
				  'material_key': '6bb50fd03269012e3526404062cdb04a',
				  'filename': 'bottom_new.stl',
				  'ref': '42',
				  'key': '128aa489f093ebdc810d706b7b98cfea',
				  'make_cost': {
					'total': '18.85',
					'making': '16.02',
					'materials': '2.83',
					'currency': 'USD'
				  }
				}
			  ]
			}";

			var result = ProductDeserializer.Deserialize(json);

			var theFirstDesign = result.Designs[0];
			Assert.AreEqual("mm", theFirstDesign.Units);

			var theBoundingBoxForTheFirstDesign = theFirstDesign.BoundingBox;
			Assert.AreEqual(14.0m, theBoundingBoxForTheFirstDesign.X);
			Assert.AreEqual(100.0m, theBoundingBoxForTheFirstDesign.Y);
			Assert.AreEqual(50.0m, theBoundingBoxForTheFirstDesign.Z);

			Assert.AreEqual(12351.5312m, theFirstDesign.Volume);
		}
	}
}
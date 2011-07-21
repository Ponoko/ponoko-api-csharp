using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutCreatingProducts : ProductAcceptanceTest {
		// TEST: you_can_create_a_product_with_multiple_designs

		[Test]
		public void can_create_a_product_provided_you_have_a_design_file_with_matching_valid_material() {
    		var expectedDesign = NewDesign();

    		var expectedNewProductName = "Any new product name";

    		var theNewProduct = Products.Save(expectedNewProductName, expectedDesign);
    		var actualDesign = theNewProduct.Designs[0];

			Assert.AreEqual(expectedNewProductName	, theNewProduct.Name, "Expected the returned product to have the name supplied.");
			Assert.IsFalse(theNewProduct.IsLocked	, "Expected the newly-created product to be unlocked.");
			Assert.IsNotNull(theNewProduct.Designs	, "Expected the product to be returned with a key.");
    		Assert.IsNotNull(theNewProduct.NodeKey	, "Expected the product to be returned with a node key.");

    		AssertIsAboutUtcNow(theNewProduct.CreatedAt, TimeSpan.FromSeconds(5));
    		AssertIsAboutUtcNow(theNewProduct.UpdatedAt, TimeSpan.FromSeconds(5));
    		
			Assert.AreEqual(1, theNewProduct.Designs.Count, "Expected the new product to have the design we supplied.");
    		AssertEqual(expectedDesign, actualDesign);
			Assert.That(actualDesign.MaterialKey, Is.Null, "Expected no material key because the product's materials are not available.");
    	}

		[Test]
    	public void you_must_supply_a_design_when_adding_a_product() {
    		Design missingDesign = null;

    		var theError = Assert.Throws<ArgumentException>(() => Products.Save("xxx", missingDesign));

			Assert.That(theError.Message, Is.StringMatching("^Cannot create a product without at least one Design\\..+"));
		}

		[Test]
    	public void you_must_supply_a_file_and_filename_with_the_design() {
			var designWithoutAFile = new Design {Filename = null};

    		var theError = Assert.Throws<ArgumentException>(() => Products.Save("xxx", designWithoutAFile));

			Assert.That(theError.Message, Is.StringMatching("^Cannot create a product unless the Design has a file\\..+"));
		}

		[Test]
    	public void you_must_supply_a_file_that_exists_on_disk_with_the_design() {
			var designWithoutAFile = new Design {Filename = "xxx_file_must_not_exist_on_disk_xxx"};

    		var theError = Assert.Throws<FileNotFoundException>(() => Products.Save("xxx", designWithoutAFile));

			Assert.That(theError.Message, Is.StringMatching(
				"^Cannot create a product unless the Design has a file that exists on disk\\..+"
			));
		}

		[Test]
    	public void you_must_supply_a_material_with_the_design() {
			var designWithoutAMaterial = new Design {Filename = "res\\bottom_new.stl", MaterialKey = null};

    		var theError = Assert.Throws<Exception>(() => Products.Save("xxx", designWithoutAMaterial));

			Assert.That(theError.Message, Is.StringMatching(
				"could not find requested material. is it available to this Node's materail catalog?"
			));
		}

		[Test]
		public void you_must_supply_a_material_that_is_compatible_with_the_design() {
			const String INVALID_MATERIAL = "6812d5403269012e2f2f404062cdb04a";

			var designWithInvalidMaterial = new Design { Filename = "res\\bottom_new.stl", MaterialKey = INVALID_MATERIAL};

    		var theError = Assert.Throws<Exception>(() => Products.Save("xxx", designWithInvalidMaterial));

			Assert.That(theError.Message, Is.StringMatching(
				"the material you have selected is not compatible with making methods available to the design file"
			));
		}

		private void AssertIsAboutUtcNow(DateTime dateTime, TimeSpan within) {
			var now = DateTime.UtcNow;
			var diff = now.Subtract(dateTime);
			Assert.That(diff, Is.LessThan(within));
		}

		private void AssertEqual(Design expected, Design actual) {
			Assert.AreEqual(Path.GetFileName(expected.Filename), actual.Filename);
			Assert.AreEqual(expected.Quantity, actual.Quantity);
			Assert.AreEqual(expected.Reference, actual.Reference);
			
			Assert.AreEqual(actual.MakeCost.Total		, 0D);
			Assert.AreEqual(actual.MakeCost.Making		, 0D);
			Assert.AreEqual(actual.MakeCost.Materials	, 0D);
			Assert.AreEqual(actual.MakeCost.Currency	, "USD");
		}

		private Design NewDesign() {
			var aValidMaterialKey = "6bb50fd03269012e3526404062cdb04a";

			return new Design {
				Filename	= new FileInfo(@"res\bottom_new.stl").FullName,
				MaterialKey = aValidMaterialKey,
				Quantity	= 1,
				Reference	= "42"
			};
		}
	}
}

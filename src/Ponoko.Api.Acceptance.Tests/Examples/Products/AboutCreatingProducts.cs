using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutCreatingProducts : ProductAcceptanceTest {
		[Test]
		public void you_can_create_a_product_provided_you_have_a_design_file_with_matching_valid_material() {
    		var expectedDesign = NewDesign();

    		var expectedNewProductName	= "Any new product name";
    		var expectedNewProductNotes = "Any new product notes";
    		var expectedNewProductRef	= Guid.NewGuid().ToString();

			var seed = new ProductSeed {
           		Name		= expectedNewProductName,
           		Notes		= expectedNewProductNotes,
           		Reference	= expectedNewProductRef
			};

    		var theNewProduct = CreateCommand.Create(seed, expectedDesign);
    		
			var actualDesign = theNewProduct.Designs[0];

			Assert.AreEqual(expectedNewProductName, theNewProduct.Name, 
				"Expected the returned product to have the name supplied."
			);
			Assert.AreEqual(expectedNewProductNotes, theNewProduct.Description, 
				"Expected the returned product to have its description set to whatever was supplied as notes."
			);
			Assert.AreEqual(expectedNewProductRef, theNewProduct.Reference, 
				"Expected the returned product to have the reference supplied."
			);
			Assert.IsFalse(theNewProduct.IsLocked, 
				"Expected the newly-created product to be unlocked."
			);
			Assert.IsNotNull(theNewProduct.Designs, 
				"Expected the product to be returned with a key."
			);
    		Assert.IsNotNull(theNewProduct.NodeKey, 
				"Expected the product to be returned with a node key."
			);

    		AssertIsAboutUtcNow(theNewProduct.CreatedAt, TimeSpan.FromSeconds(5));
    		AssertIsAboutUtcNow(theNewProduct.UpdatedAt, TimeSpan.FromSeconds(5));
    		
			Assert.AreEqual(1, theNewProduct.Designs.Count, 
				"Expected the new product to have the design we supplied."
			);
    		AssertEqual(expectedDesign, actualDesign);
			Assert.IsNull(actualDesign.MaterialKey, 
				"Expected no material key because the product's materials are not available."
			);
    	}

		[Test]
		public void you_can_create_a_product_with_multiple_designs() {
			var firstDesign = NewDesign();
			var secondDesign = NewDesign();

			var seed = new ProductSeed {
           		Name		= "Any new product name",
           		Notes		= "Any new product notes",
           		Reference	= Guid.NewGuid().ToString()
			};

    		var theNewProduct = CreateCommand.Create(seed, firstDesign, secondDesign);

			Assert.AreEqual(2, theNewProduct.Designs.Count, 
				"Expected that because two designs were supplied, the resultant product should also contain two designs."
			);
		}

		[Test] 
		public void after_a_product_is_created_it_can_be_fetched_ie_it_has_been_persisted() {
			var aNewProduct = CreateCommand.Create(ProductSeed.WithName("A new product"), NewDesign());

			var theProductFetchedById = new FindCommand(Internet, Settings.BaseUrl).Find(aNewProduct.Key);

			Assert.AreEqual(theProductFetchedById.Key, aNewProduct.Key, 
				"The product was fetched successfully, but it has an unexpected key."
			);
			Assert.AreEqual(theProductFetchedById.Name, aNewProduct.Name, 
				"The product was fetched successfully, but it has an unexpected name."
			);
		}

		[Test]
    	public void you_must_supply_a_design_when_adding_a_product() {
    		Design missingDesign = null;

    		var theError = Assert.Throws<ArgumentException>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), missingDesign));

			Assert.That(theError.Message, Is.StringMatching("^Cannot create a product without at least one Design\\..+"));
		}

		[Test]
    	public void you_must_supply_a_file_and_filename_with_the_design() {
			var designWithoutAFile = new Design {Filename = null};

    		var theError = Assert.Throws<ArgumentException>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), designWithoutAFile));

			Assert.That(theError.Message, Is.StringMatching("^Cannot create a product unless the Design has a file\\..+"));
		}

		[Test]
    	public void you_must_supply_a_file_that_exists_on_disk_with_the_design() {
			var designWithoutAFile = new Design {Filename = "xxx_file_must_not_exist_on_disk_xxx"};

    		var theError = Assert.Throws<FileNotFoundException>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), designWithoutAFile));

			Assert.That(theError.Message, Is.StringMatching(
				"^Cannot create a product unless the Design has a file that exists on disk\\..+"
			));
		}

		[Test]
    	public void you_must_supply_a_material_with_the_design() {
			var designWithoutAMaterial = new Design {Filename = "res\\bottom_new.stl", MaterialKey = null};

    		var theError = Assert.Throws<Exception>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), designWithoutAMaterial));

			Assert.That(theError.Message, Is.StringMatching(
				"could not find requested material. is it available to this Node's materail catalog?"
			));
		}

		[Test]
		public void you_must_supply_a_material_that_is_compatible_with_the_design() {
			const String INVALID_MATERIAL = "6812d5403269012e2f2f404062cdb04a";

			var designWithInvalidMaterial = new Design { Filename = "res\\bottom_new.stl", MaterialKey = INVALID_MATERIAL};

    		var theError = Assert.Throws<Exception>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), designWithInvalidMaterial));

			Assert.That(theError.Message, Is.StringMatching(
				"the material you have selected is not compatible with making methods available to the design file"
			));
		}
		
		[Test]
		public void you_must_supply_a_material_that_is_available() {
			const String MATERIAL_THAT_DOES_NOT_EXIST = "1337";

			var designWithInvalidMaterial = new Design { Filename = "res\\bottom_new.stl", MaterialKey = MATERIAL_THAT_DOES_NOT_EXIST};

    		var theError = Assert.Throws<Exception>(() => CreateCommand.Create(ProductSeed.WithName("xxx"), designWithInvalidMaterial));

			Assert.That(theError.Message, Is.StringMatching(
				"could not find requested material. is it available to this Node's materail catalog?"
			));
		}

		[Test]
		public void you_must_supply_a_name_but_notes_and_ref_are_optional() {
			var theError = Assert.Throws<ArgumentException>(() => CreateCommand.Create(ProductSeed.WithName(null), NewDesign()), 
				"Expected an error when trying to create a product with null name."
			);

			Assert.That(theError.Message, Is.StringMatching(
				"Cannot create a product without a name."
			));
			
			theError = Assert.Throws<ArgumentException>(() => CreateCommand.Create(ProductSeed.WithName("\r\n\t "), NewDesign()), 
				"Expected an error when trying to create a product with empty name."
			);

			Assert.That(theError.Message, Is.StringMatching(
				"Cannot create a product without a name."
			));

			Product productWithoutNotesOrReference = null;

			Assert.DoesNotThrow(() => productWithoutNotesOrReference = CreateCommand.Create(ProductSeed.WithName("Any non-empty name"), NewDesign()), 
				"Both notes and reference are optional, so did not expect a validation error."
			);

			Assert.IsEmpty(productWithoutNotesOrReference.Description, 
				"Expected that creating a product with null notes should result in a product with empty string as a description"
			);

			Assert.IsEmpty(productWithoutNotesOrReference.Reference, 
				"Expected that creating a product with null notes should result in a product with empty string as a reference"
			);
		}

		[Test]
		public void you_must_supply_a_unique_reference() {
			var anyDesign	= NewDesign();

			var seed = new ProductSeed {
				Name		= "Any new product name", 
				Notes		= "Any notes", 
				Reference	= Guid.NewGuid().ToString()
			};

			CreateCommand.Create(seed, anyDesign);	

			var theError = Assert.Throws<Exception>(() => CreateCommand.Create(seed, anyDesign));	
			Assert.That(theError.Message, Is.StringContaining("'Ref' must be unique"));
		}

		private void AssertIsAboutUtcNow(DateTime expected, TimeSpan within) {
			var now = DateTime.UtcNow;
			var diff = now.Subtract(expected);
			Assert.That(diff, Is.LessThan(within), 
				"Expected <{0}> to be with about <{1}> of <{2}>, but the difference is <{3}>", 
				expected, 
				within,
				now, 
				diff
			);
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
			const String VALID_MATERIAL_KEY = "6bb50fd03269012e3526404062cdb04a";

			return new Design {
				Filename	= new FileInfo(@"res\bottom_new.stl").FullName,
				MaterialKey = VALID_MATERIAL_KEY,
				Quantity	= 1,
				Reference	= "42"
			};
		}
	}
}

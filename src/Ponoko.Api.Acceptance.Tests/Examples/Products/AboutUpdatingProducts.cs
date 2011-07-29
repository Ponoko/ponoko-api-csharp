using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutUpdatingProducts : ProductAcceptanceTest {		
		[SetUp]
		public void SetUp() {
			ExampleProduct = NewProduct("A product to update");	
		}

		[TearDown]
		public void TearDown() {
			Delete(ExampleProduct);
		}

		[Test]
		public void you_can_update_the_main_details_for_a_product() {
			var command = new UpdateCommand(Internet, Settings.BaseUrl, new DefaultProductValidator());
			
			var theUpdate = new ProductSeed {
				Name		= "The updated name",
				Notes		= "The updated notes",
				Reference	= "The updated reference"
			};

			var result = command.Update(ExampleProduct.Key, theUpdate);
			
			Assert.AreEqual(theUpdate.Name, result.Name, "Expected the name to have been updated");
			Assert.AreEqual(theUpdate.Notes, result.Description, 
				"Expected the notes to have been updated, and returned as description (not notes)"
			);
			Assert.AreEqual(theUpdate.Reference, result.Reference, "Expected the reference to have been updated");
		}

		[Test]
		public void updating_a_product_sets_its_updated_timestamp_and_leaves_its_created_timestamp_alone() {
			var initialUpdatedAt = ExampleProduct.UpdatedAt;
			var initialCreatedAt = ExampleProduct.CreatedAt;
			
			var command = new UpdateCommand(Internet, Settings.BaseUrl, new DefaultProductValidator());
			
			var theUpdate = new ProductSeed { Name = "xxx"};

			var result = command.Update(ExampleProduct.Key, theUpdate);
			
			Assert.That(result.UpdatedAt, Is.GreaterThan(initialUpdatedAt), 
				"Expected then updated_at timestamp to have been reset on update"
			);

			Assert.That(result.CreatedAt, Is.EqualTo(initialCreatedAt), 
				"Expected then created_at timestamp to have retained its initial value"
			);
		}

		[Test]
		public void you_can_add_a_new_design() {
			var theInitialDesignCount = ExampleProduct.Designs.Count;

			var command = new AddDesignCommand(Internet, Settings.BaseUrl);
			var theNewDesign = NewDesign();

			var result = command.Add(ExampleProduct.Key, theNewDesign);

			Assert.AreEqual(theInitialDesignCount + 1, result.Designs.Count, "Expected the new design to have been added");
		}

		[Test]
		public void you_can_update_an_existing_design_for_example_you_can_change_its_material() {
			var command = new AddDesignCommand(Internet, Settings.BaseUrl);
			
			var theDesign = ExampleProduct.Designs[0];
			var theOriginalMaterial = theDesign.MaterialKey;

			theDesign.MaterialKey = ExampleMaterials.SUPERFINE_PLASTIC;

			var result = command.Update(ExampleProduct.Key, theDesign);

			var theUpdatedDesign = result.Designs[0];

			Assert.AreEqual(ExampleMaterials.SUPERFINE_PLASTIC, theUpdatedDesign.MaterialKey, 
				"Expected the design's material key to have been updated"
			);

			Assert.AreNotEqual(theOriginalMaterial, theUpdatedDesign.MaterialKey, 
				"Expected the design's material key to have been changed from what it was"
			);
		}

		[Test] 
		public void you_can_only_change_the_file_name_when_uploading_a_new_design_file() {
			var command = new AddDesignCommand(Internet, Settings.BaseUrl);
			
			var theDesign = ExampleProduct.Designs[0];

			var theNewFile = new FileInfo("res\\another_bottom_new.stl");

			var result = command.Update(ExampleProduct.Key, theDesign, theNewFile);

			var theUpdatedDesign = result.Designs[0];	

			Assert.AreEqual(theNewFile.Name, theUpdatedDesign.Filename, "Expected the filename to have been updated");
		}

		[Test]
		public void you_can_delete_a_design() {
			var theLastDesign = ExampleProduct.Designs[0];
			var addNewDesign = new AddDesignCommand(Internet, Settings.BaseUrl);
			
			ExampleProduct = addNewDesign.Add(ExampleProduct.Key, NewDesign());

			Assert.AreEqual(2, ExampleProduct.Designs.Count, 
				"In order for this test to be valid, the product needs to have " + 
				"more than one design (since you can't delete the last one)"
			);

			var command = new DeleteDesignCommand(Internet, Settings.BaseUrl);

			command.Delete(ExampleProduct.Key, ExampleProduct.Designs[1].Key);

			var theRefreshedProduct = new FindCommand(Internet, Settings.BaseUrl).Find(ExampleProduct.Key);

			Assert.AreEqual(1, theRefreshedProduct.Designs.Count, "Expected the desing to have been deleted");
			Assert.AreEqual(theLastDesign.Key, theRefreshedProduct.Designs[0].Key, "Expected that the newly-added one was deleted");
		}

		[Test]
		public void you_cannot_delete_the_last_design() {
			var command = new DeleteDesignCommand(Internet, Settings.BaseUrl);
			Assert.Throws<Exception>(() => command.Delete(ExampleProduct.Key, ExampleProduct.Designs[0].Key), 
				"Products are only valid while they have designs and you can't delete the last design from a product."
			);
		}

		private Product ExampleProduct { get; set; }
	}
}

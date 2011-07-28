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

		private Product ExampleProduct { get; set; }
	}
}

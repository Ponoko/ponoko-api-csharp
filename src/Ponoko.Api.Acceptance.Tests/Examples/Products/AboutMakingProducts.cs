using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Core.Orders.Commands;
using Ponoko.Api.Core.Shipping.Repositories;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutMakingProducts : OrderingAcceptanceTest {
		[Test]
		public void you_can_get_the_shipping_options_for_a_single_product() {
			var result = new ShippingOptionsRepository(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency code returned.");
			Assert.AreEqual(ExampleProduct.Key, result.Products[0].Key, "Unexpected key returned.");
			
			Assert.That(result.Options.Count, Is.GreaterThan(0), "Unexpected at least one option.");
			Assert.That(result.Options[0].Price, Is.GreaterThan(0.00d), "Unexpected price for the first option, expected non-zero value.");
		}

		[Test]
		public void you_can_get_the_shipping_options_for_multiple_products() {
			var multipleProducts = new [] {ExampleShippingInfo, ExampleShippingInfo};
			var result = new ShippingOptionsRepository(Internet, Settings.BaseUrl).For(ExampleAddress, multipleProducts);

			Assert.AreEqual(2, result.Products.Count, "Expected number of products returned");
		}

		[Test]
		public void you_can_for_example_ship_to_the_united_kingdom() {
			var result = new ShippingOptionsRepository(Internet, Settings.BaseUrl).For(TenDowningStreet, ExampleShippingInfo);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency code returned.");
			Assert.AreEqual(ExampleProduct.Key, result.Products[0].Key, "Unexpected key returned.");
			
			Assert.That(result.Options.Count, Is.GreaterThan(0), "Unexpected at least one option.");
			Assert.That(result.Options[0].Price, Is.GreaterThan(0.00d), "Unexpected price for the first option, expected non-zero value.");
		}

		[Test]
		public void you_can_get_a_product_made() {
			var shippingOptions = new ShippingOptionsRepository(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);
			var command = new OrderHistory(Internet, Settings.BaseUrl);
			
			var theFirstShippingOption = shippingOptions.Options[0];
			var reference = Guid.NewGuid().ToString();

			var order = command.Create(reference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo);

			Assert.AreEqual(ExampleProduct.Key, order.Products.First().Key, "Unexpected key returned");
			Assert.AreEqual("order_received", order.Events.First().Name, "Expected the returned order to have an order_created event");
		}

		[Test]
		public void to_get_a_product_made_you_must_supply_a_unique_reference() {
			var shippingOptions = new ShippingOptionsRepository(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);
			var command = new OrderHistory(Internet, Settings.BaseUrl);
			
			var theFirstShippingOption = shippingOptions.Options[0];
			var duplicateReference = Guid.NewGuid().ToString();

			Assert.DoesNotThrow(() => 
				command.Create(duplicateReference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo), 
                "The first time a reference is used the order should be created successfully"                
			);

			var theError = Assert.Throws<Exception>(() => 
				command.Create(duplicateReference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo)
			);

			Assert.That(theError.Message, Is.StringContaining("'Ref' must be unique"));
		}

		// TEST: you_have_to_supply_a_product
		// TEST: you_can_order_more_that_one
		// TEST: you_get_invalid_oauth_request_unless_you_supply_state
	}
}

using System;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Core.Shipping;
using FindCommand = Ponoko.Api.Core.Shipping.Commands.FindCommand;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutMakingAProduct : ProductAcceptanceTest {
		private Product _exampleProduct;

		[TestFixtureTearDown]
		public void TestFixtureTearDown() {
			Assert.Throws<Exception>(() => 
				new DeleteCommand(Internet, Settings.BaseUrl).Delete(ExampleProduct.Key), 
                "Expected this to fail because the delete operation is broken on the server end. " + 
                "It didn't fail, so the remote end has been fixed and you can remove this assertion."
			);
		}

		[Test]
		public void you_can_get_the_shipping_options_for_a_single_product() {
			var result = new FindCommand(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency code returned.");
			Assert.AreEqual(ExampleProduct.Key, result.Products[0].Key, "Unexpected key returned.");
			
			Assert.That(result.Options.Count, Is.GreaterThan(0), "Unexpected at least one option.");
			Assert.That(result.Options[0].Price, Is.GreaterThan(0.00d), "Unexpected price for the first option, expected non-zero value.");
		}

		[Test]
		public void you_can_get_the_shipping_options_for_multiple_products() {
			var multipleProducts = new [] {ExampleShippingInfo, ExampleShippingInfo};
			var result = new FindCommand(Internet, Settings.BaseUrl).For(ExampleAddress, multipleProducts);

			Assert.AreEqual(2, result.Products.Count, "Expected number of products returned");
		}

		[Test]
		public void you_can_for_example_ship_to_the_united_kingdom() {
			var result = new FindCommand(Internet, Settings.BaseUrl).For(TenDowningStreet, ExampleShippingInfo);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency code returned.");
			Assert.AreEqual(ExampleProduct.Key, result.Products[0].Key, "Unexpected key returned.");
			
			Assert.That(result.Options.Count, Is.GreaterThan(0), "Unexpected at least one option.");
			Assert.That(result.Options[0].Price, Is.GreaterThan(0.00d), "Unexpected price for the first option, expected non-zero value.");
		}

		private Product ExampleProduct {
			get { return _exampleProduct ?? (_exampleProduct = GetFirstProduct()); }
		}

		private Address ExampleAddress {
			get { 
				return new Address {
					LineOne			= "27 Dixon Street",
					LineTwo			= "Te Aro",
					City			= "Wellington",
					ZipOrPostalCode = "6021",
					State			= "NA",
					Country			= "New Zealand"
			    };
			}
		}

		private Address TenDowningStreet {
			get { 
				return new Address {
					LineOne			= "10 Downing Street",
					LineTwo			= "Westminster",
					City			= "London",
					ZipOrPostalCode = "SW1A 2AA",
					State			= "NA",
					Country			= "United Kingdom"
			    };
			}
		}

		public ProductShippingInfo ExampleShippingInfo { 
			get {
				return new ProductShippingInfo {
					Key = ExampleProduct.Key,
					Quantity = 1
				};
			}
		}

		private Product GetFirstProduct() {
			given_at_least_one_product("An example for purchasing");

			var theKey = FindFirstProductKey();

			return new Core.Product.Commands.FindCommand(Internet, Settings.BaseUrl).Find(theKey);
		}

		// TEST: you_have_to_supply_a_product
		// TEST: you_can_order_more_that_one
		// TEST: you_get_invalid_oauth_request_unless_you_supply_state
	}
}

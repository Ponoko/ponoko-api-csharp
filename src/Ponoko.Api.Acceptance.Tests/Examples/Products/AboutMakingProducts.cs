using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Shipping;
using Ponoko.Api.Core.Shipping.Commands;
using FindCommand = Ponoko.Api.Core.Shipping.Commands.FindCommand;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutMakingProducts : ProductAcceptanceTest {
		[SetUp]
		public void SetUp() {
			ExampleProduct = NewProduct("A product to for making");	
		}

		[TearDown]
		public void TearDown() {
			Delete(ExampleProduct);
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

		[Test]
		public void you_can_get_a_product_made() {
			var shippingOptions = new FindCommand(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);
			var command = new OrderCreateCommand(Internet, Settings.BaseUrl);
			
			var theFirstShippingOption = shippingOptions.Options[0];
			var reference = Guid.NewGuid().ToString();

			var order = command.Create(reference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo);

			Assert.AreEqual(ExampleProduct.Key, order.Products.First().Key, "Unexpected key returned");
		}

		[Test]
		public void to_get_a_product_made_reference_must_be_unique() {
			var shippingOptions = new FindCommand(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);
			var command = new OrderCreateCommand(Internet, Settings.BaseUrl);
			
			var theFirstShippingOption = shippingOptions.Options[0];
			var duplicateReference = Guid.NewGuid().ToString();

			Assert.DoesNotThrow(() 
				=> command.Create(duplicateReference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo), 
                "The first time a reference is used the order should be created successfully"                
			);

			var theError = Assert.Throws<Exception>(()
				=> command.Create(duplicateReference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo)
			);

			Assert.That(theError.Message, Is.StringContaining("'Ref' must be unique"));
		}

		private NameAndAddress ExampleShippingAddress {
			get { 
				return new NameAndAddress {
					FirstName		= "Jazz",
					LastName		= "Kang",
					LineOne			= "27 Dixon Street",
					LineTwo			= "Te Aro",
					City			= "Wellington",
					ZipOrPostalCode = "6021",
					State			= "NA",
					Country			= "NZ",
					Phone			= "Any telephone number"
			    };
			}
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

		private Product ExampleProduct { get; set; }

		// TEST: you_have_to_supply_a_product
		// TEST: you_can_order_more_that_one
		// TEST: you_get_invalid_oauth_request_unless_you_supply_state
	}
}

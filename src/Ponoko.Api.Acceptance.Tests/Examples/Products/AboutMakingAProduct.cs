﻿using System;
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
			var result = new FindCommand(Internet, Settings.BaseUrl).For(ExampleShippingInfo);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency code returned.");
			Assert.AreEqual(ExampleProduct.Key, result.Products[0].Key, "Unexpected key returned.");
			
			Assert.AreEqual(2, result.Options.Count, "Unexpected number of options.");
			Assert.That(result.Options[0].Price, Is.GreaterThan(0.00d), "Unexpected price for the first option, expected non-zero value.");
		}

		[Test]
		public void you_can_get_the_shipping_options_for_multiple_products() {
			var multipleProducts = new [] {ExampleShippingInfo, ExampleShippingInfo};
			var result = new FindCommand(Internet, Settings.BaseUrl).For(multipleProducts);

			Assert.AreEqual(2, result.Products.Count, "Expected number of products returned");
		}

		private Product ExampleProduct {
			get { return _exampleProduct ?? (_exampleProduct = GetFirstProduct()); }
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
	}
}

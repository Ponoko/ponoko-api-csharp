using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Core.Orders;
using Ponoko.Api.Core.Orders.Repositories;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Core.Shipping;
using Ponoko.Api.Core.Shipping.Repositories;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDeletingProducts : ProductAcceptanceTest {
		[Test]
		public void you_can_delete_a_product() {
			var product = NewProduct("A product for testing deletes");

			new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key);

			then_the_product_does_not_exist_with_key(product.Key);
		}
		
		[Test, Ignore("TEMP: cannot delete orders, or products with orders")]
		public void you_can_delete_them_all() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var delete = new DeleteCommand(Internet, Settings.BaseUrl);
			var result = finder.FindAll();

			var count = 0;

			Console.WriteLine("Deleting {0} products", result.Length);

			foreach (var product in result) {
				delete.Delete(product.Key);
				count++;
			}

			Console.WriteLine("Deleted {0} products", count);

			Assert.AreEqual(0, finder.FindAll().Length, "Expected them all to have been deleted");
		}

	    [Test]
	    public void you_cannot_delete_a_product_that_has_orders()
	    {
	        var product = NewProduct("A product for testing deletes");
	        
            CreateANewOrder(product);
	        
            var theError = Assert.Throws<Exception>(() => new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key));

            Assert.That(theError.Message, Is.StringMatching("cannot delete product that has orders"));
	    }

	    private Order CreateANewOrder(Product product)
        {
            var command = new OrderRepository(Internet, Settings.BaseUrl);

            var reference = Guid.NewGuid().ToString();

            var shippingInfo = new ProductShippingInfo() { Key = product.Key, Quantity = 1};

            return command.Create(reference, AnyShippingOption(shippingInfo), ExampleShippingAddress, shippingInfo);
        }

        private Option AnyShippingOption(ProductShippingInfo shipping)
        {
            var allOptions = new ShippingOptionsRepository(Internet, Settings.BaseUrl).
                For(ExampleAddress, shipping);

            return allOptions.Options.First();
        }

        protected NameAndAddress ExampleShippingAddress
        {
            get
            {
                return new NameAndAddress
                {
                    FirstName = "Jazz",
                    LastName = "Kang",
                    LineOne = "27 Dixon Street",
                    LineTwo = "Te Aro",
                    City = "Wellington",
                    ZipOrPostalCode = "6021",
                    State = "NA",
                    Country = "NZ",
                    Phone = "Any telephone number"
                };
            }
        }

        protected Address ExampleAddress
        {
            get
            {
                return new Address
                {
                    LineOne = "27 Dixon Street",
                    LineTwo = "Te Aro",
                    City = "Wellington",
                    ZipOrPostalCode = "6021",
                    State = "NA",
                    Country = "New Zealand"
                };
            }
        }

        protected Address TenDowningStreet
        {
            get
            {
                return new Address
                {
                    LineOne = "10 Downing Street",
                    LineTwo = "Westminster",
                    City = "London",
                    ZipOrPostalCode = "SW1A 2AA",
                    State = "NA",
                    Country = "United Kingdom"
                };
            }
        }

		private void then_the_product_does_not_exist_with_key(String id) {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			var result = finder.Find(id);

			Assert.IsNull(result, "Expected that finding the product with key <{0}> would return null.", id);
		}

		// [Test] public void it_deletes_all_products_with_the_same_key() { }
		// [Test] public void cannot_delete_a_product_that_has_been_made() { }
	}
}

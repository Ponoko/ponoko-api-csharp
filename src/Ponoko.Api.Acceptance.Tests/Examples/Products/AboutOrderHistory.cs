using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ponoko.Api.Core.Orders;
using Ponoko.Api.Core.Orders.Commands;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Core.Shipping;
using Ponoko.Api.Core.Shipping.Commands;
using System.Linq;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutOrderHistory : OrderingAcceptanceTest {
		public Order NewOrder { get; set; }
		
		[SetUp]
		public void TestFixtureSetUp() {
			NewOrder = CreateANewOrder();
		}

		[Test]
		public void you_can_get_the_list_of_orders() {
			var result = new OrderHistory(Internet, Settings.BaseUrl).FindAll();

			AssertTheOrderIsReturned(result, NewOrder.Reference);
		}

		[Test]
		public void you_cannot_delete_an_order() {
			var uri = Map("/orders/delete/{0}", NewOrder.Key);

			var theError = Assert.Throws<Exception>(() 
				=> new DeleteResourceCommand(Internet, Settings.BaseUrl).Delete(uri)
			);

			Assert.That(theError.Message, Is.StringMatching("^Delete failed. The server returned status NotFound \\(404\\).+"));
		}

		private void AssertTheOrderIsReturned(IEnumerable<Order> orders, String reference) {
			var exists = orders.Any(it => it.Reference == reference);

			Assert.IsTrue(exists, "Could not find an order with reference <{0}>", reference);
		}

		private Order CreateANewOrder() {
			var command = new OrderCreateCommand(Internet, Settings.BaseUrl);
			
			var reference = Guid.NewGuid().ToString();

			return command.Create(reference, AnyShippingOption(), ExampleShippingAddress, ExampleShippingInfo);
		}

		private Option AnyShippingOption() {
			var allOptions = new FindShippingOptionsCommand(Internet, Settings.BaseUrl).
				For(ExampleAddress, ExampleShippingInfo);

			return allOptions.Options.First();
		}
	}
}

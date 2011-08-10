using System;
using Ponoko.Api.Core.Shipping.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutOrderHistory : OrderingAcceptanceTest {
		[Test]
		public void you_can_get_the_list_of_orders() {
			var shippingOptions = new FindCommand(Internet, Settings.BaseUrl).For(ExampleAddress, ExampleShippingInfo);
			var command = new OrderCreateCommand(Internet, Settings.BaseUrl);
			
			var theFirstShippingOption = shippingOptions.Options[0];
			var reference = Guid.NewGuid().ToString();

			var order = command.Create(reference, theFirstShippingOption, ExampleShippingAddress, ExampleShippingInfo);

			var finder = new FindCommand(Internet, Settings.BaseUrl);
			
			var result = finder.All();

			Assert.Fail(
				"Even though the query was successful, " + 
				"this can't be tested until we're able to create orders."
			);
		}
	}
}

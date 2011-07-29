using NUnit.Framework;
using Ponoko.Api.Core.Orders;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutOrderHistory : ProductAcceptanceTest {
		[Test]
		public void you_can_get_the_list_of_orders() {
			var finder = new FindCommand(Internet, Settings.BaseUrl);
			
			var result = finder.All();

			Assert.Fail(
				"Even though the query was successful, " + 
				"this can't be tested until we're able to create orders."
			);
		}
	}
}

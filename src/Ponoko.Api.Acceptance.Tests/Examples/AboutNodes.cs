using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Repositories;

namespace Ponoko.Api.Acceptance.Tests.Examples {
    [TestFixture]
    public class AboutNodes : AcceptanceTest {
    	[Test]
        public void you_can_get_the_list_of_available_making_nodes() {
			var nodes = new Nodes(Internet, Settings.BaseUrl);

    		var all = nodes.FindAll();

			Assert.That(all.Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}
    }
}

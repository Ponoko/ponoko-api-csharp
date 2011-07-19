using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples {
    [TestFixture]
    public class AboutNodes : AcceptanceTest {
    	[Test]
        public void you_can_get_the_list_of_available_making_nodes() {
			var authorizationPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
				Settings.Credentials
			);
			
			var theInternet = new SystemInternet(authorizationPolicy);
			var nodes = new Nodes(theInternet, Settings.BaseUrl);

    		var all = nodes.FindAll();

			Assert.Less(0, all.Count, "Expected at least one making node to be returned.");
    	}
    }
}

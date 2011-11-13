using NUnit.Framework;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Logging;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;
using Ponoko.Api.Rest.Security.Simple;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	public class About_authentication : AcceptanceTest {
    	[Test]
        public void you_can_use_oauth() {
			var authPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(
					new SystemClock(),
					new SystemNonceFactory()
				),
				Settings.Credentials
			);
				
			var theInternetWithOAuth =  new SystemInternet(authPolicy, new ConsoleLog());

			var nodes = new Nodes(theInternetWithOAuth, Settings.BaseUrl);

    		Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}

		[Test]
        public void you_can_use_simple_key_authorization() {
			var authPolicy = new SimpleKeyAuthorization(Settings.SimpleKeyAuthorizationCredential);
				
			var theInternetWithSimpleKeyAuthorization =  new SystemInternet(authPolicy, new ConsoleLog());

			var nodes = new Nodes(theInternetWithSimpleKeyAuthorization, Settings.BaseUrl);

			Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}
	}
}

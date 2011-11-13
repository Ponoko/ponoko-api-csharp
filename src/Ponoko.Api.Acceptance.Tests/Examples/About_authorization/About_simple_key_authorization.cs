using NUnit.Framework;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Logging;
using Ponoko.Api.Rest.Security.Simple;

namespace Ponoko.Api.Acceptance.Tests.Examples.About_authorization {
	public class About_simple_key_authorization : AcceptanceTest {
		[Test]
        public void you_can_use_simple_key_authorization() {
			var authPolicy = new SimpleKeyAuthorizationPolicy(Settings.SimpleKeyAuthorizationCredential);
				
			var theInternetWithSimpleKeyAuthorization =  new SystemInternet(authPolicy, new ConsoleLog());

			var nodes = new Nodes(theInternetWithSimpleKeyAuthorization, Settings.BaseUrl);

			Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}

	}
}

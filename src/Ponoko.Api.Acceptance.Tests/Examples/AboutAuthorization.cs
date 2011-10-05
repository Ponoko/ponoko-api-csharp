using System;
using NUnit.Framework;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	public class AboutAuthorization : AcceptanceTest {
		[Test]
		public void if_you_supply_invalid_credentials_you_get_an_error() {
			var invalidCredentials = new CredentialSet(new Credential("xxx_clearly_invalid", ""));
			
			var authorizationPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
				invalidCredentials 
			);
			
			var theInternet = new SystemInternet(authorizationPolicy);
			var nodes = new Nodes(theInternet, Settings.BaseUrl);

			var theError = Assert.Throws<Exception>(() => nodes.FindAll());

			Assert.That(theError.Message, Is.StringEnding("The server returned status Unauthorized (401), and error message: \"Invalid OAuth Request\""));
		}
	}
}

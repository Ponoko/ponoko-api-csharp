﻿using System;
using NUnit.Framework;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Logging;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples.About_authorization {
	[TestFixture]
	public class About_oauth {
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
	}
}

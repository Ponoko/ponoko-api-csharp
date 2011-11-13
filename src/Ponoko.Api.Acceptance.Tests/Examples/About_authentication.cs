using System;
using NUnit.Framework;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

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
				
			var theInternetWithOAuth =  new SystemInternet(authPolicy);

			var nodes = new Nodes(theInternetWithOAuth, Settings.BaseUrl);

    		Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}

		[Test]
        public void you_can_use_simple_key_authorization() {
			var authPolicy = new SimpleKeyAuthorization(Settings.SimpleKeyAuthorizationCredential);
				
			var theInternetWithsimpleKeyauthorization =  new SystemInternet(authPolicy);

			var nodes = new Nodes(theInternetWithsimpleKeyauthorization, Settings.BaseUrl);

			Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}
	}

	internal class SimpleKeyAuthorizationTests {
		[Test]
		public void it_adds_the_key_to_the_url() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorization(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/")));
			Request authorized = simpleKeyAuthorizer.Authorize(request);

			var expected = new Uri("http://xxx/?app_key=abcdefgh&user_access_key=stuvwxyz");
			Assert.AreEqual(expected, authorized.RequestLine.Uri);
		}

		// [Test] it_preserves_other_params
		// [Test] it_fails_if_url_already_contains_either_parameter
	}

	public class SimpleKeyAuthorizationCredential {
		public readonly string AppKey;
		public readonly string UserAccessKey;

		public SimpleKeyAuthorizationCredential(String appKey, String userAccessKey) {
			AppKey = appKey;
			UserAccessKey = userAccessKey;
		}
	}

	public class SimpleKeyAuthorization : AuthorizationPolicy {
		private readonly SimpleKeyAuthorizationCredential _credential;

		public SimpleKeyAuthorization(SimpleKeyAuthorizationCredential credential) {
			_credential = credential;
		}

		public Request Authorize(Request request) {
			var newUri = new Uri(
				request.RequestLine.Uri + 
				"?app_key=" + _credential.AppKey + 
				"&user_access_key=" + _credential.UserAccessKey
			);
			var newRequestLine = new RequestLine(request.RequestLine.Verb, newUri, request.RequestLine.Version);
			return new Request(newRequestLine, request.Headers, request.Payload);
		}
	}
}

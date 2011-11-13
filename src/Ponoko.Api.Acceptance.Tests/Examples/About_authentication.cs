using System;
using System.Text;
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
				
			var theInternetWithSimpleKeyAuthorization =  new SystemInternet(authPolicy);

			var nodes = new Nodes(theInternetWithSimpleKeyAuthorization, Settings.BaseUrl);

			Assert.That(nodes.FindAll().Count, Is.GreaterThan(0), "Expected at least one making node to be returned.");
    	}
	}

	public class SimpleKeyAuthorizationTests {
		[Test]
		public void it_adds_the_key_to_the_url() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorization(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/")));
			var authorized = simpleKeyAuthorizer.Authorize(request);

			var expected = new Uri("http://xxx/?app_key=abcdefgh&user_access_key=stuvwxyz");
			Assert.AreEqual(expected, authorized.RequestLine.Uri);
		}

		[Test] 
		public void it_preserves_other_params() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorization(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/?a=b")));
			var authorized = simpleKeyAuthorizer.Authorize(request);

			var expected = new Uri("http://xxx/?a=b&app_key=abcdefgh&user_access_key=stuvwxyz");
			Assert.AreEqual(expected, authorized.RequestLine.Uri);
		}

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
			var originalParams = request.RequestLine.Parameters;
			originalParams.Add(new Parameter { Name = "app_key", Value = _credential.AppKey });
			originalParams.Add(new Parameter { Name = "user_access_key", Value = _credential.UserAccessKey});

			var theQueryString = new StringBuilder();

			foreach (var parameter in originalParams) {
				theQueryString.AppendFormat("{0}={1}&", parameter.Name, parameter.Value);
			}

			var theBaseUri = new UriBuilder(
				request.RequestLine.Uri.Scheme, 
				request.RequestLine.Uri.Host,
			    request.RequestLine.Uri.Port
			);

			var newUri = new Uri(theBaseUri + "?" + theQueryString.ToString().TrimEnd('&'));

			var newRequestLine = new RequestLine(request.RequestLine.Verb, newUri, request.RequestLine.Version);
			
			return new Request(newRequestLine, request.Headers, request.Payload);
		}
	}
}

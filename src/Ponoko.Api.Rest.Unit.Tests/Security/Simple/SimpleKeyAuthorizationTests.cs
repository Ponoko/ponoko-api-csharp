using System;
using NUnit.Framework;
using Ponoko.Api.Rest.Security.Simple;

namespace Ponoko.Api.Rest.Unit.Tests.Security.Simple {
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
}
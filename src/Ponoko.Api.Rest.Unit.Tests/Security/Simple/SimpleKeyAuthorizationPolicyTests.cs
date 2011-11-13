﻿using System;
using NUnit.Framework;
using Ponoko.Api.Rest.Security.Simple;

namespace Ponoko.Api.Rest.Unit.Tests.Security.Simple {
	public class SimpleKeyAuthorizationPolicyTests {
		[Test]
		public void it_adds_the_app_key_and_user_access_key_to_the_url() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorizationPolicy(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/")));
			var authorized = simpleKeyAuthorizer.Authorize(request);

			var expected = new Uri("http://xxx/?app_key=abcdefgh&user_access_key=stuvwxyz");
			Assert.AreEqual(expected, authorized.RequestLine.Uri);
		}

		[Test] 
		public void it_preserves_other_params() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorizationPolicy(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/?a=b")));
			var authorized = simpleKeyAuthorizer.Authorize(request);

			var expected = new Uri("http://xxx/?a=b&app_key=abcdefgh&user_access_key=stuvwxyz");
			Assert.AreEqual(expected, authorized.RequestLine.Uri);
		}

		[Test] 
		public void it_preserves_the_path() {
			var simpleKeyAuthorizer = new SimpleKeyAuthorizationPolicy(
				new SimpleKeyAuthorizationCredential("abcdefgh", "stuvwxyz")
			);

			var request = new Request(RequestLine.Get(new Uri("http://xxx/yyy/zzz")));
			var authorized = simpleKeyAuthorizer.Authorize(request);

			Assert.AreEqual(request.RequestLine.Uri.AbsolutePath, authorized.RequestLine.Uri.AbsolutePath);
		}

		// [Test] it_fails_if_url_already_contains_either_parameter
	}
}
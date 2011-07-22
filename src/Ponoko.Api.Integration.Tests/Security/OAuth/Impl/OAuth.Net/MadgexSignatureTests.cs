using System;
using System.Collections.Specialized;
using NUnit.Framework;
using OAuth.Net.Common;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Integration.Tests.Security.OAuth.Impl {
	[TestFixture]
	public class MadgexSignatureTests {
		[Test]
		public void it_signs_a_get_with_no_params() {
			var instance = new MadgexSignature();
			var expected = "D5udKZ1nfB1a/tthTrw/5jK+1b4=";

			var request = new Request(RequestLine.Get(new Uri("http://xxx/"))) {
				ContentType = "application/x-www-form-urlencoded"
			};

			var actual = instance.Sign(
				request, 
				"secret", 
				"token_secret", 
				AnyParameters
			);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void it_signs_a_get_with_params() {
			var instance = new MadgexSignature();
			var expected = "L8KuASdRJ768AuiRKueMJbPJF9k=";

			var p = AnyParameters;
			p.AdditionalParameters.Add(new NameValueCollection {{"xxx", "xxx_value"}});

			var request = new Request(RequestLine.Get(new Uri("http://xxx/"))) {
				ContentType = "application/x-www-form-urlencoded"
			};

			var actual = instance.Sign(
				request,
				"secret", 
				"token_secret", 
				p
			);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void it_signs_a_post_with_no_params() {
			var instance = new MadgexSignature();
			var expected = "H99gAVVpNjn9dGRq/YTpAOO9BTE=";

			var request = new Request(RequestLine.Post(new Uri("http://xxx/"))) {
				ContentType = "application/x-www-form-urlencoded"
			};

			var actual = instance.Sign(
				request,
				"secret", 
				"token_secret", 
				AnyParameters
			);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void it_signs_a_post_with_params() {
			var instance = new MadgexSignature();
			var expected = "kAn1ws3K4V5iv2zHLfz9z4ivHgA=";

			var p = AnyParameters;
			p.AdditionalParameters.Add(new NameValueCollection {{"xxx", "xxx_value"}});

			var request = new Request(RequestLine.Post(new Uri("http://xxx/"))) {
				ContentType = "application/x-www-form-urlencoded"
			};

			var actual = instance.Sign(
				request,
				"secret", 
				"token_secret", 
				p
			);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void when_signing_a_post_it_does_not_include_parameters_in_the_signature_when_the_content_type_is_multipart() {
			var instance = new MadgexSignature();
			var expected = "H99gAVVpNjn9dGRq/YTpAOO9BTE=";

			var p = AnyParameters;
			p.AdditionalParameters.Add(new NameValueCollection {{"xxx", "xxx_value"}});

			var request = new Request(RequestLine.Post(new Uri("http://xxx/"))) {
				ContentType = "multipart/form-data, boundary=xxx_boundary_xxx"
			};

			var actual = instance.Sign(
				request,
				"secret", 
				"token_secret", 
				p
			);

			Assert.AreEqual(expected, actual);
		}

		private OAuthParameters AnyParameters {
			get {
				return new OAuthParameters {
                   	ConsumerKey = "key",
                   	Token = "token",
                   	Timestamp = "1309746190",
                   	Nonce = "1309746190",
                   	Version = "1.0",
                   	SignatureMethod = "HMAC-SHA1"
				};
			}
		}
	}
}

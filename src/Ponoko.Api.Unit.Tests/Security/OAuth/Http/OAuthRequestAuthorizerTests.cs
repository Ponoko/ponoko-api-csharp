using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Ponoko.Api.Security.OAuth.Core;
using Ponoko.Api.Security.OAuth.Http;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Security.OAuth.Http {
	[TestFixture]
	public class OAuthRequestAuthorizerTests {
		private OAuthHeader _oAuthHeaderProvider;
		private readonly CredentialSet _anyCredentials = new CredentialSet(new Credential("key", "secret"));
		private CredentialSet AnyCredentials { get { return _anyCredentials; } }

		[SetUp]
		public void SetUp() {
			_oAuthHeaderProvider = MockRepository.GenerateStub<OAuthHeader>();
			_oAuthHeaderProvider.
				Stub(r => r.New(Arg<Request>.Is.Anything, Arg<CredentialSet>.Is.Anything)).
				Return("A fake OAuth header");
		}

		[Test]
		public void it_adds_an_oauth_header() {
			var request = Request.Get(new Uri("http://xxx"));
			
			var oAuthRequestAuthorizer = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials);

			var result = oAuthRequestAuthorizer.Authorize(request);
			
			Assert.That(result.Headers.Count, Is.GreaterThan(0));
		}

		[Test]
		public void it_uses_its_oauth_header_once_to_generate_the_header() {
			var request = Request.Get(new Uri("http://xxx"));

			new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials).Authorize(request);
			
			_oAuthHeaderProvider.AssertWasCalled(
				provider => provider.New(
					Arg<Request>.Is.Equal(request), 
					Arg<CredentialSet>.Is.Equal(AnyCredentials)
				),
				options => options.Repeat.Once()
			); 
		}
		
		[Test]
		public void it_preserves_supplied_uri() {
			var request = Request.Get(new Uri("http://xxx?jazz=a%20fat%20tart"));

			var result = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials).Authorize(request);

			Assert.AreEqual(request.RequestLine.Uri, result.RequestLine.Uri, 
				"Expected that the initial URI stay the same (implying query parameters are not parsed)"
			);
		}

		[Test]
		public void it_does_not_parse_query_string_from_uri() {
			var request = Request.Get(new Uri("http://xxx?jazz=a%20fat%20tart"));

			var result = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials).Authorize(request);

			Assert.IsNull(result.Payload.Parameters["jazz"], 
				"Expected that the returned parameters NOT include the one we put in the query string."
			);
		}

		[Test]
		public void it_preserves_supplied_parameters() {
			var anyTwat = "Phil Murphy";
			var anyOtherTwat = "Jazz Kang";

			var parameters = new NameValueCollection {
				{anyTwat, "Gluten-free anything"},
				{anyOtherTwat, "DIY kebab"} 
			};

			var request = Request.Get(new Uri("http://xxx"), parameters);

			var result = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials).Authorize(request);

			Assert.That(result.Payload.Parameters, Contains.Item(anyTwat));	
			Assert.That(result.Payload.Parameters, Contains.Item(anyOtherTwat));	
		}

		[Test]
		public void it_fails_with_null_credentials() {
			var request = Request.Get(new Uri("http://xxx"));
			CredentialSet nullCredentials = null;
			
			var oAuthRequestAuthorizer = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, nullCredentials);

			var theError = Assert.Throws<InvalidOperationException>(() => 
				oAuthRequestAuthorizer.Authorize(request)
			);

			Assert.AreEqual("Credentials are required.", theError.Message);
		}
	}
}

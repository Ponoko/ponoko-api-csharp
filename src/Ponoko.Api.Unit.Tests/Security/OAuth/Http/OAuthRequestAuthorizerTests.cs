using System;
using NUnit.Framework;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
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
		public void when_authorizing_a_get_it_leaves_parameters_in_uri_and_does_not_return_payload() {
			var uri = new Uri("http://xxx?Phil%20Murphy=Gluten-free%20anything&Jazz%20Kang&DIY%Kebab");

			var request = Request.Get(uri);

			var result = new OAuthAuthorizationPolicy(_oAuthHeaderProvider, AnyCredentials).Authorize(request);

			Assert.AreEqual(uri, result.RequestLine.Uri);
			Assert.That(result.Payload, Is.Empty, "Expected no payload because the request was a GET");	
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

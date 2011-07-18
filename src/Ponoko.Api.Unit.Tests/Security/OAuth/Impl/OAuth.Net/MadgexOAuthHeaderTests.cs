using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Security.OAuth.Impl.OAuth.Net {
    [TestFixture]
    public class MadgexOAuthHeaderTests {
    	private Clock _clock;
    	private NonceFactory _nonceFactory;

    	private readonly CredentialSet _anyConsumerAndToken = new CredentialSet(
    		new Credential("key", "secret"),
    		new Credential("token_key", "token_secret")
		);

    	[SetUp]
		public void BeforeEach() {
			_clock = MockRepository.GenerateStub<Clock>();			
			_nonceFactory = MockRepository.GenerateStub<NonceFactory>();			
		}

    	[Test]
        public void can_generate_correct_header_without_token() {
            var request = Request.Get(new Uri("http://xxx/"));
			request.ContentType = "application/x-www-form-urlencoded";
            var consumer = new CredentialSet(new Credential("key", "secret"));

            _clock.Stub(clock => clock.NewTimestamp()).Return("1303687141");
            _nonceFactory.Stub(clock => clock.NewNonce()).Return("38a2dd30277558668a92913686175bb1");

            var instance = new MadgexOAuthHeader(_clock, _nonceFactory);
            var result = instance.New(request, consumer);

            Assert.That(result, Contains.Substring("oauth_signature=\"lKUJBmz5ULuNh67y8KRGrWRRMvI%3D\""));
        }

        [Test]
        public void can_generate_correct_header_with_token() {
            var request = Request.Get(new Uri("http://xxx/"));
			request.ContentType = "application/x-www-form-urlencoded";

            _clock.Stub(clock => clock.NewTimestamp()).Return("1303688247");
            _nonceFactory.Stub(clock => clock.NewNonce()).Return("0fa90c9e1b3c035d650aed6d991b6e0c");

            var instance = new MadgexOAuthHeader(_clock, _nonceFactory);
            var result = instance.New(request, _anyConsumerAndToken);

            Assert.That(result, Contains.Substring("oauth_signature=\"jaD4HW0CDjnIuJrZv0FKLylfC4w%3D\""));
        }

        [Test]
        public void can_generate_correct_header_with_parameters() {
            var parameters = new NameValueCollection{ {"name", "value"} };
            var request = Request.Get(new Uri("http://xxx/"), new NameValueCollection(), parameters);
        	request.ContentType = "application/x-www-form-urlencoded";

        	_clock.Stub(clock => clock.NewTimestamp()).Return("1303692000");
        	_nonceFactory.Stub(clock => clock.NewNonce()).Return("ba3af980256a87ba503c19ef91863c5e");

            var instance = new MadgexOAuthHeader(_clock, _nonceFactory);
            var result = instance.New(request, _anyConsumerAndToken);

            Assert.That(result, Contains.Substring("oauth_signature=\"fZ9JbVXALa5PHFGfpV66qJ7gi2Q%3D\""));
        }

        [Test]
        public void can_generate_correct_ssl_header() {
            var request = Request.Get(new Uri("https://xxx/"));
			request.ContentType = "application/x-www-form-urlencoded";

            _clock.Stub(clock => clock.NewTimestamp()).Return("1303707043");
            _nonceFactory.Stub(clock => clock.NewNonce()).Return("99e2d65a57e2b74f92ddf1f23fbf6393");

			var instance = new MadgexOAuthHeader(_clock, _nonceFactory);
            var result = instance.New(request, _anyConsumerAndToken);

            Assert.That(result, Contains.Substring("oauth_signature=\"aWPeOt4T8gB03CrvFnQn7LL3vKA%3D\""));
        }

        [Test]
        public void requests_a_new_nonce_and_new_timestamp_with_each_invocation() {
            var request = Request.Get(new Uri("http://xxx/?name=value"));
			request.ContentType = "application/x-www-form-urlencoded";
            var consumerAndToken = new CredentialSet(Credential.Empty, Credential.Empty);

            var mockClock = MockRepository.GenerateStub<Clock>();
            mockClock.Stub(clock => clock.NewTimestamp()).Return("1303692000");
            var mockNonce = MockRepository.GenerateStub<NonceFactory>();
            mockNonce.Stub(clock => clock.NewNonce()).Return("ba3af980256a87ba503c19ef91863c5e");

            var instance = new MadgexOAuthHeader(mockClock, mockNonce);

            instance.New(request, consumerAndToken);
            instance.New(request, consumerAndToken);

            mockClock.AssertWasCalled(clock => clock.NewTimestamp(), options => options.Repeat.Twice());
            mockNonce.AssertWasCalled(nonce => nonce.NewNonce(), options => options.Repeat.Twice());
        }
    }
}
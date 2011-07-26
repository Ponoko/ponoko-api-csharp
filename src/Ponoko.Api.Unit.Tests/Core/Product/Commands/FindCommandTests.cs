using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core.Product.Commands {
	[TestFixture]
	public class FindCommandTests : DomainUnitTest {
		[Test]
		public void find_returns_null_when_internet_returns_404_not_found() {
			var notFoundResponse = MockRepository.GenerateStub<Response>();
			notFoundResponse.Stub(it => it.StatusCode).Return(HttpStatusCode.NotFound);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(notFoundResponse);

			var finder = new FindCommand(internet, "http://xxx");

			var result = finder.Find("xxx");

			Assert.IsNull(result);
		}

		[Test]
		public void find_fails_when_internet_returns_401_unauthorized() {
			var expectedStatus = HttpStatusCode.Unauthorized;
			
			var unauthorizedResponse = NewFakeResponse(expectedStatus);

			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(unauthorizedResponse);

			var finder = new FindCommand(internet, "http://xxx");

			var expectedError = String.Format(
				"Authorization failed. The server returned status {0} ({1}).", 
				expectedStatus, 
				(Int32)expectedStatus
			);

			var theError = Assert.Throws<Exception>(() => finder.Find("xxx"));

			Assert.AreEqual(expectedError, theError.Message, "The error was raised, but the message does not match.");
		}

		[Test]
		public void exists_returns_a_value_that_depends_on_response_status() {
			var okResponse = NewFakeResponse(HttpStatusCode.OK);
			var internet = MockRepository.GenerateStub<TheInternet>();
			var finder = new FindCommand(internet, "http://xxx");
			
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(okResponse);
			Assert.IsTrue(finder.Exists("xxx"), "Expected true because the internet returned OK.");

			var notFoundResponse = NewFakeResponse(HttpStatusCode.NotFound);
			internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(notFoundResponse).Repeat.AtLeastOnce();
			
			finder = new FindCommand(internet, "http://xxx");
			Assert.IsFalse(finder.Exists("xxx"), "Expected false because the internet returned NotFound.");
		}

		[Test]
		public void exists_fails_when_internet_returns_401_unauthorized() {
			var expectedStatus = HttpStatusCode.Unauthorized;
			
			var unauthorizedResponse = NewFakeResponse(expectedStatus);

			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(unauthorizedResponse);

			var finder = new FindCommand(internet, "http://xxx");

			var expectedError = String.Format(
				"Authorization failed. The server returned status {0} ({1}).", 
				expectedStatus, 
				(Int32)expectedStatus
			);

			var theError = Assert.Throws<Exception>(() => finder.Exists("xxx"));

			Assert.AreEqual(expectedError, theError.Message, "The error was raised, but the message does not match.");
		}

		[Test]
		public void find_all_fails_when_internet_returns_401_unauthorized() {
			var expectedStatus = HttpStatusCode.Unauthorized;
			
			var unauthorizedResponse = NewFakeResponse(expectedStatus);

			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(unauthorizedResponse);

			var finder = new FindCommand(internet, "http://xxx");

			var expectedError = String.Format(
				"Authorization failed. The server returned status {0} ({1}).", 
				expectedStatus, 
				(Int32)expectedStatus
			);

			var theError = Assert.Throws<Exception>(() => finder.FindAll());

			Assert.AreEqual(expectedError, theError.Message, "The error was raised, but the message does not match.");
		}

		// TEST: exists_invokes_head_instead_of_get_since_we_do_not_need_the_body
	}
}

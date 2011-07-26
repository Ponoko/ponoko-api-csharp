using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core.Product.Commands {
	[TestFixture]
	public class DeleteCommandTests : DomainUnitTest {
	[Test]
		public void it_returns_okay_when_delete_succeeds() {
			var deleteSuccessfulBody = "{'product_key': '1234', 'deleted': 'true'}";
			var okayResponse = NewFakeResponse(HttpStatusCode.OK, deleteSuccessfulBody);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).Return(okayResponse);

			var deleter = new DeleteCommand(internet, AnyUrl);
			
			Assert.DoesNotThrow(() => deleter.Delete("any id"));
		}

		[Test]
		public void it_fails_when_the_server_responds_with_delete_failed_message() {
			var deleteSuccessfulBody = "{'product_key': '1234', 'deleted': 'false'}";
			var okayResponse = NewFakeResponse(HttpStatusCode.OK, deleteSuccessfulBody);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).Return(okayResponse);

			var deleter = new DeleteCommand(internet, AnyUrl);
			
			var theError = Assert.Throws<Exception>(() => deleter.Delete("any id"));

			var expectedError = "Delete failed. Expected the deleted flag to be true. but it was \"false\".";

			Assert.AreEqual(expectedError, theError.Message, "An error was raised as expected, but the message does not match.");
		}

		private string AnyUrl { get { return "http://xxx/"; } }
	}
}

using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core.Product.Commands {
	[TestFixture]
	public class DeleteResourceCommandTests : DomainUnitTest {
		[Test]
		public void it_returns_okay_provided_the_internet_returns_200_OK_and_the_deleted_flag() {
			var internet = MockRepository.GenerateStub<TheInternet>();
			
			var response = NewFakeResponse(
				HttpStatusCode.OK, 
				"{'deleted' : 'true'}"
			);

			internet.
				Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<HttpContentType>.Is.Anything, Arg<Payload>.Is.Anything)).
				Return(response);
			
			var command = new DeleteResourceCommand(internet, "http://xxx");
			
			Assert.DoesNotThrow(() => command.Delete(new Uri("http://xxx")));
		}

		[Test]
		public void it_fails_when_the_internet_returns_a_failed_delete() {
			var internet = MockRepository.GenerateStub<TheInternet>();
			
			var response = NewFakeResponse(
				HttpStatusCode.OK, 
				"{'deleted' : 'false'}"
			);

			internet.
				Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<HttpContentType>.Is.Anything, Arg<Payload>.Is.Anything)).
				Return(response);
			
			var command = new DeleteResourceCommand(internet, "http://xxx");
			
			var theError = Assert.Throws<Exception>(() => command.Delete(new Uri("http://xxx")));

			Assert.AreEqual("Delete failed. Expected the deleted flag to be true. but it was \"false\".", theError.Message);
		}

		[Test]
		public void it_fails_when_the_internet_does_not_return_the_deleted_flag_at_all() {
			var internet = MockRepository.GenerateStub<TheInternet>();

			var invalidResponseBody = 
				"{\r\n  " + 
					"\"name\": \"Mark Unsworth\",\r\n  " + 
					"\"nickname\": \"Skid\"\r\n" + 
				"}";
			
			var response = NewFakeResponse(
				HttpStatusCode.OK, 
				invalidResponseBody
			);

			internet.
				Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<HttpContentType>.Is.Anything, Arg<Payload>.Is.Anything)).
				Return(response);
			
			var command = new DeleteResourceCommand(internet, "http://xxx");
			
			var theError = Assert.Throws<Exception>(() => command.Delete(new Uri("http://xxx")));

			var expected = String.Format(
				"Delete failed. " + 
			    "Expected the response to contain a deleted flag, but it does not. " + 
			    "The server returned: {0}", 
				invalidResponseBody
			);

			Assert.AreEqual(expected, theError.Message, "An error was raised, but it has an unexpected message");
		}
	}
}

using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core {
	[TestFixture]
	public class ProductsTests : DomainUnitTest {
		[Test]
		public void it_fails_to_save_unless_internet_responds_with_200() {
			var internet = MockRepository.GenerateMock<TheInternet>();
			
			var fileSystem = NewFakeValidator();

			var expectedName = "Any product name";
			var expectedStatus = HttpStatusCode.InternalServerError;
			var expectedErrorMessage = "xxx_error_xxx";

			var response = NewFakeResponse(
				expectedStatus, 
				String.Format("{{ error : {{ message : \"{0}\", \"errors\" : [] }}}}", expectedErrorMessage)
			);

			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).
				Repeat.Once().
				Return(response);

			var products = new Products(internet, AnyUrl, fileSystem);

			var theError = Assert.Throws<Exception>(() => products.Create(ProductSeed.WithName(expectedName), AnyDesign()));
			
			var expectedError = String.Format(
				"Failed to save product. " +
				"The server returned status {0} ({1}), and error message: \"{2}\"", 
				expectedStatus, 
				(Int32)expectedStatus,
				expectedErrorMessage
			);

			Assert.That(theError.Message, Is.EqualTo(expectedError));
		}

		[Test]
		public void it_returns_okay_when_delete_succeeds() {
			var deleteSuccessfulBody = "{'product_key': '1234', 'deleted': 'true'}";
			var okayResponse = NewFakeResponse(HttpStatusCode.OK, deleteSuccessfulBody);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).Return(okayResponse);

			var products = new Products(internet, AnyUrl, NewFakeValidator());
			
			Assert.DoesNotThrow(() => products.Delete("any id"));
		}

		[Test]
		public void it_fails_when_the_server_responds_with_delete_failed_message() {
			var deleteSuccessfulBody = "{'product_key': '1234', 'deleted': 'false'}";
			var okayResponse = NewFakeResponse(HttpStatusCode.OK, deleteSuccessfulBody);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).Return(okayResponse);

			var products = new Products(internet, AnyUrl, NewFakeValidator());
			
			var theError = Assert.Throws<Exception>(() => products.Delete("any id"));

			var expectedError = "Delete failed. Expected the deleted flag to be true. but it was \"false\".";

			Assert.AreEqual(expectedError, theError.Message, "An error was raised as expected, but the message does not match.");
		}

		[Test]
		public void it_fails_with_an_error_that_contains_raw_response_text_if_error_cannot_be_parsed() {
			var clearlyNotJson = "Rex Boppington went to Kilbirnie";
			var failedFailedResponse = NewFakeResponse(HttpStatusCode.InternalServerError, clearlyNotJson);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).Return(failedFailedResponse);

			var products = new Products(internet, AnyUrl, NewFakeValidator());

			var theError =  Assert.Throws<Exception>(() => products.Create(ProductSeed.WithName("xxx"), AnyDesign()));

			var expectedError = String.Format(
				"There was a problem deserializing the error message. " + 
				"The body of the response is: {0}", clearlyNotJson
			);

			Assert.That(theError.Message, Is.StringMatching(expectedError), 
				"Expected the exception message to contain the body of the response"
			);

			Assert.That(theError.InnerException, Is.Not.Null, "Expected the exception to have an inner exception");
		}

		private Design AnyDesign() {
			return new Design {
          		Filename = "xxx",
          		MaterialKey = "xxx",
          		Quantity = 1,
          		Reference = "xxx"
			};
		}

		public string AnyUrl { get { return "http://xxx/"; } }

		private ProductValidator NewFakeValidator() {
			var validator = MockRepository.GenerateMock<ProductValidator>();
			validator.Stub(it => it.Validate(Arg<ProductSeed>.Is.Anything));
			validator.Stub(it => it.Validate(Arg<Design[]>.Is.Anything));
			return validator;
		}

		// TEST: it rejects products without designs
		// TEST: what does save return?
		// TEST: when_save_fails_it_failes_with_message_that_includes_each_error_message
	}
}

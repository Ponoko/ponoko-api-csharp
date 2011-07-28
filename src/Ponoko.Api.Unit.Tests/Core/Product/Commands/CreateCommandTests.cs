using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core.Product.Commands {
	[TestFixture]
	public class CreateCommandTests : DomainUnitTest {
		[Test]
		public void it_fails_to_create_unless_internet_responds_with_200() {
			var internet = MockRepository.GenerateMock<TheInternet>();
			
			var fileSystem = NewFakeValidator();

			var expectedName = "Any product name";
			var expectedStatus = HttpStatusCode.InternalServerError;
			var expectedErrorMessage = "xxx_error_xxx";

			var response = NewFakeResponse(
				expectedStatus, 
				String.Format("{{ error : {{ message : \"{0}\", \"errors\" : [] }}}}", expectedErrorMessage)
			);

			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<HttpContentType>.Is.Anything, Arg<Payload>.Is.Anything)).
				Repeat.Once().
				Return(response);

			var products = new CreateCommand(internet, AnyUrl, fileSystem);

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
		public void it_fails_with_an_error_that_contains_raw_response_text_if_error_cannot_be_parsed() {
			var clearlyNotJson = "Rex Boppington went to Kilbirnie";
			var failedFailedResponse = NewFakeResponse(HttpStatusCode.InternalServerError, clearlyNotJson);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<HttpContentType>.Is.Anything, Arg<Payload>.Is.Anything)).Return(failedFailedResponse);

			var products = new CreateCommand(internet, AnyUrl, NewFakeValidator());

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

		private string AnyUrl { get { return "http://xxx/"; } }

		private ProductValidator NewFakeValidator() {
			var validator = MockRepository.GenerateMock<ProductValidator>();
			validator.Stub(it => it.Validate(Arg<ProductSeed>.Is.Anything));
			validator.Stub(it => it.Validate(Arg<Design[]>.Is.Anything));
			return validator;
		}
	}
}

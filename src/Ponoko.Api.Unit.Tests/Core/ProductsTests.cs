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
			
			var expectedStatus = HttpStatusCode.InternalServerError;
			var expectedErrorMessage = "xxx_error_xxx";

			var response = NewFakeResponse(expectedStatus, String.Format("{{ error : {{ message : \"{0}\" }}}}", expectedErrorMessage));

			internet.Stub(it => it.Post(Arg<Uri>.Is.Anything, Arg<Payload>.Is.Anything)).
				Repeat.Once().
				Return(response);

			var aFakeNewProduct = AnyProduct();

			var products = new Products(internet, "http://xxx/");

			var theError = Assert.Throws<Exception>(() => products.Save(aFakeNewProduct));
			
			var expectedError = String.Format(
				"Failed to save product. " +
				"The server returned status {0} ({1}), and error message: \"{2}\"", 
				expectedStatus, 
				(Int32)expectedStatus,
				expectedErrorMessage
			);

			Assert.That(theError.Message, Is.EqualTo(expectedError));
		}

		private Product AnyProduct() {
			var aFakeNewProduct = new Product { Name = "Fake" };
			var design = new Design {
          		Filename = "xxx",
          		MaterialKey = "xxx",
          		Quantity = 1,
          		Reference = "xxx"
			};
			
			aFakeNewProduct.Designs.Add(design);
			
			return aFakeNewProduct;
		}

		// TEST: it sends each design associated with the Product
		// TEST: it rejects products without designs
	}
}

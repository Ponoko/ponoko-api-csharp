using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core {
	[TestFixture]
	public class ProductFinderTests {
		[Test]
		public void it_returns_null_when_internet_returns_404_not_found() {
			var notFoundResponse = MockRepository.GenerateStub<Response>();
			notFoundResponse.Stub(it => it.StatusCode).Return(HttpStatusCode.NotFound);
			
			var internet = MockRepository.GenerateStub<TheInternet>();
			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(notFoundResponse);

			var finder = new ProductFinder(internet, "http://xxx");

			var result = finder.Find("xxx");

			Assert.IsNull(result);
		}
	}
}

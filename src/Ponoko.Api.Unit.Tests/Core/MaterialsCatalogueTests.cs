using System;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Repositories;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core {
	[TestFixture]
	public class MaterialsCatalogueTests : DomainUnitTest {
		[Test]
		public void it_queries_the_internet() {
			var internet = MockRepository.GenerateMock<TheInternet>();
			
			internet.Expect(it => it.Get(Arg<Uri>.Is.Anything)).
				Repeat.Once().
				Return(NewFakeResponse(HttpStatusCode.OK, "{\"materials\": []}")).
				Message("Expected FindAll to query the internet.");
			
			new MaterialsCatalogue(internet, "http://xxx/").FindAll("xxx");
		}

		[Test]
		public void it_disposes_of_the_response_returned_from_the_internet() {
			var internet = MockRepository.GenerateMock<TheInternet>();

			var theResponse = NewFakeResponse(HttpStatusCode.OK, "{\"materials\": []}");

			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(theResponse).Repeat.Once();

			new MaterialsCatalogue(internet, "http://xxx/").FindAll("xxx");

			theResponse.AssertWasCalled(it => it.Dispose(), options => 
				options.Repeat.Once().Message("Expected the response to be disposed of after use.")
			);
		}
	}
}

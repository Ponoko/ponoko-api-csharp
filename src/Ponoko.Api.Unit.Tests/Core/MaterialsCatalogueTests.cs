using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Rest;
using Ponoko.Api.Security.OAuth.Core;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core {
	[TestFixture]
	public class MaterialsCatalogueTests {
		[Test]
		public void it_queries_the_internet() {
			var internet = MockRepository.GenerateMock<TheInternet>();
			
			internet.Expect(it => it.Get(Arg<Uri>.Is.Anything)).
				Repeat.Once().
				Return(NewFakeResponse()).
				Message("Expected FindAll to query the internet.");
			
			new MaterialsCatalogue(internet, "http://xxx/").FindAll("xxx");
		}

		[Test]
		public void it_disposes_of_the_response_returned_from_the_internet() {
			var internet = MockRepository.GenerateMock<TheInternet>();

			var theResponse = NewFakeResponse();

			internet.Stub(it => it.Get(Arg<Uri>.Is.Anything)).Return(theResponse).Repeat.AtLeastOnce();

			new MaterialsCatalogue(internet, "http://xxx/").FindAll("xxx");

			theResponse.AssertWasCalled(it => it.Dispose(), options => 
				options.Repeat.Once().Message("Expected the response to be disposed of after use.")
			);
		}

		private Response NewFakeResponse() {
			var fakeResponse = MockRepository.GenerateMock<Response>();
			fakeResponse.Stub(it => it.Open()).Return(SomeReadableJson("{\"materials\": []}"));
			return fakeResponse;
		}

		private MemoryStream SomeReadableJson(String json) {
			var theBytes = Encoding.UTF8.GetBytes(json);
			var fakeJson = new MemoryStream(theBytes.Length);
			fakeJson.Write(theBytes , 0, theBytes.Length);
			fakeJson.Seek(0, SeekOrigin.Begin);
			return fakeJson;
		}
	}
}

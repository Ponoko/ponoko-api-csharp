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
			var fakeResponse = MockRepository.GenerateMock<Response>();
			fakeResponse.Stub(it => it.GetResponseStream()).Return(SomeReadableJson("{\"materials\": []}"));

			var internet = MockRepository.GenerateMock<TheInternet>();
			
			internet.Expect(it => it.Get(Arg<Uri>.Is.Anything)).
				Return(fakeResponse).
				Message("Expected FindAll to query the internet.");
			
			new MaterialsCatalogue(internet, "http://xxx/").FindAll("xxx");
		}

		private MemoryStream SomeReadableJson(String json) {
			var fakeJson = new MemoryStream(256);
			var theBytes = Encoding.UTF8.GetBytes(json);
			fakeJson.Write(theBytes , 0, theBytes.Length);
			fakeJson.Seek(0, SeekOrigin.Begin);
			return fakeJson;
		}
	}
}

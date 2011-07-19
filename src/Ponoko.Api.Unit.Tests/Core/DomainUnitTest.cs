using System;
using System.IO;
using System.Net;
using System.Text;
using Ponoko.Api.Rest;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Core {
	public class DomainUnitTest {
		protected Response NewFakeResponse(HttpStatusCode status, String body) {
			var result = MockRepository.GenerateMock<Response>();
			
			result.Stub(it => it.Open()).Return(SomeReadableJson(body));
			result.Stub(it => it.StatusCode).Return(status);

			return result;
		}

		protected Stream SomeReadableJson(String json) {
			var theBytes = Encoding.UTF8.GetBytes(json);
			var fakeJson = new MemoryStream(theBytes.Length);
			fakeJson.Write(theBytes , 0, theBytes.Length);
			fakeJson.Seek(0, SeekOrigin.Begin);
			return fakeJson;
		}
	}
}
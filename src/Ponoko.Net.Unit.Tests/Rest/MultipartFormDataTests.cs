using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using Ponoko.Net.Rest;
using Rhino.Mocks;

namespace Ponoko.Net.Unit.Tests.Rest {
	[TestFixture]
	public class MultipartFormDataTests {
		public void 
		the_content_type_includes_a_boundary() {
			var fakeRequest = MockRepository.GenerateMock<IHttpRequest>();
			var instance = new MultipartFormData();
			var parameters = new NameValueCollection();
			var noDataItems = new List<DataItem>(0);

			instance.WriteBody(fakeRequest, new Payload(parameters));
		}

		// TEST: it_writes_each_parameter_as_expected
		// TEST: it_writes_its_body_to_the_web_requests_stream
		// TEST: and_that_boundary_goes_between_each_paramater
		// TEST: you_can_also_write_binary_data
	}
}

using System;
using System.Collections.Specialized;
using NUnit.Framework;

namespace Ponoko.Net.Unit.Tests.Rest {
	[TestFixture]
	public class GetTests {
		[Test] public void 
		it_adds_parameters_to_its_request_line_uri() {
			var parameters = new NameValueCollection {{"xxx", "yyy"}};

			var get = new Get(new Uri("http://xxx/"), parameters);

			Assert.AreEqual("http://xxx/?xxx=yyy", get.RequestLine.Uri.AbsoluteUri);
		}

		[Test] public void 
		it_url_encodes_its_parameters() {
			var parameters = new NameValueCollection {{"name", "Phil Murphy plays the blues"}};
			
			var get = new Get(new Uri("http://xxx/"), parameters);

			Assert.AreEqual("http://xxx/?name=Phil%20Murphy%20plays%20the%20blues", get.RequestLine.Uri.AbsoluteUri);
		}

		[Test] public void 
		it_adds_a_slash_to_the_uri_if_missing() {
			var get = new Get(new Uri("http://xxx"));

			Assert.AreEqual("http://xxx/", get.RequestLine.Uri.AbsoluteUri);
		}

		[Test] public void 
		it_leaves_extra_slashes_alone() {
			var get = new Get(new Uri("http://xxx//"));

			Assert.AreEqual("http://xxx//", get.RequestLine.Uri.AbsoluteUri);
		}

		[Test] public void 
		it_has_an_empty_body() {
			var get = new Get(new Uri("http://xxx"));
			Assert.AreEqual(RequestBody.None, get.Body);
		}
	}
}

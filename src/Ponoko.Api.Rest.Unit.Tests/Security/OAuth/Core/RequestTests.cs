using System;
using NUnit.Framework;

namespace Ponoko.Api.Rest.Unit.Tests.Security.OAuth.Core {
	[TestFixture]
	public class RequestTests {
		[Test]
		public void it_leaves_query_string_in_uri_if_there_is_one() {
			var requestLine = new RequestLine("GET", new Uri("http://xxx?name=value"));
			var instance = new Request(requestLine);
			Assert.That(instance.RequestLine.Uri, Is.EqualTo(new Uri("http://xxx?name=value")), "Expected the Uri to stay the same");
		}

		[Test]
		public void you_can_ask_for_the_query_parameters() {
			var requestLine = new RequestLine("GET", new Uri("http://xxx?name=value"));
			var instance = new Request(requestLine);
			var theParameters = instance.RequestLine.Parameters;

			Assert.AreEqual(1, theParameters.Count, "Expected one parameters");
			Assert.AreEqual("value", theParameters[0].Value, "Unexpected value for the first parameter");
		}
	}

	[TestFixture]
	public class RequestLineTests {
		[Test]
		public void what_happens_when_a_parameter_is_in_the_query_string_with_the_same_name_twice() {
			var requestLine = new RequestLine("GET", new Uri("http://xxx?name=value&name=value_1"));
			var theParameters = requestLine.Parameters;

			Assert.AreEqual(2, theParameters.Count, "Expected two separate parameters, one for each instance in the query string");
			
			Assert.AreEqual("name"		, theParameters[0].Name, "Unexpected name for the first item");
			Assert.AreEqual("value"		, theParameters[0].Value, "Unexpected value for the first item");
			Assert.AreEqual("name"		, theParameters[1].Name, "Unexpected name for the second item");
			Assert.AreEqual("value_1"	, theParameters[1].Value, "Unexpected value for the seciond item");
		}
	}
}

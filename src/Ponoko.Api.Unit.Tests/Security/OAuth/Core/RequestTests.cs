using System;
using NUnit.Framework;
using Ponoko.Api.Rest.Security.OAuth.Core;

namespace Ponoko.Api.Unit.Tests.Security.OAuth.Core {
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
			Assert.AreEqual("value", theParameters["name"], "Expected one parameters");
		}
	}
}

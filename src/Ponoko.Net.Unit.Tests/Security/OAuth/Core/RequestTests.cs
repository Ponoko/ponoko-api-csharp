using System;
using NUnit.Framework;
using Ponoko.Net.Security.OAuth.Core;

namespace Ponoko.Net.Unit.Tests.Security.OAuth.Core {
	[TestFixture]
	public class RequestTests {
		[Test]
		public void it_ignores_parameters_supplied_in_query_string() {
			var requestLine = new RequestLine("GET", new Uri("http://xxx?name=value"));

			var instance = new Request(requestLine);
			Assert.False(instance.HasAnyParameters);

			instance.Payload.Parameters.Add("xxx", "xxx_value");
			Assert.True(instance.HasAnyParameters);
			Assert.That(instance.Payload.Parameters["xxx"], Is.EqualTo("xxx_value"));
		}

		[Test]
		public void it_leaves_query_string_in_uri_if_there_is_one() {
			var requestLine = new RequestLine("GET", new Uri("http://xxx?name=value"));
			var instance = new Request(requestLine);
			Assert.That(instance.RequestLine.Uri, Is.EqualTo(new Uri("http://xxx?name=value")), "Expected the Uri to stay the same");
		}
	}
}

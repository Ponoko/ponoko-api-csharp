using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests {
	[TestFixture]
	public class Characterization : AcceptanceTest {
		[Test]
		public void can_post_a_file_and_it_emerges_with_correct_size() {
			var parameters = new List<Parameter> {
			    new Parameter { Name = "SUBMIT", Value = "Upload!"}, 
			    new Parameter { Name = "xxx", Value = "xxx"}, 
			};

			var uri = new Uri("http://www.toledorocket.com/perftest/uploadtest/uploadstatus.asp");

			var theFile = new FileInfo(@"res\not_an_eps_really.eps");
			var theExpectedFileSizeInBytes = theFile.Length;

			var dataItems = new List<DataItem> {
			    new DataItem("FILE1", theFile, "text/plain")
			};

			var payload = new Payload(parameters, dataItems);

			using (var response = Post(uri, payload)) {
				var body = Body(response);

				var expectedMessage = String.Format("You uploaded {0} bytes", theExpectedFileSizeInBytes);

				Assert.That(body, Is.StringContaining(expectedMessage));
			}
		}
	}
}

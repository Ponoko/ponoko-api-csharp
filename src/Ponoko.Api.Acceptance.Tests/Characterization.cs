using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Acceptance.Tests {
	[TestFixture]
	public class Characterization : AcceptanceTest {
		[Test]
		public void can_post_a_file_and_it_emerges_with_correct_size() {
			var theFile = new FileInfo(@"res\not_an_eps_really.eps");
			
			var fields = new List<Field> {
			    new Field { Name = "SUBMIT", Value = "Upload!"}, 
			    new Field { Name = "xxx", Value = "xxx"}, 
				new Field { Name = "xxx", Value =  new DataItem("FILE1", theFile, "text/plain") }
			};

			var payload = new Payload(fields);
			
			var uri = new Uri("http://www.toledorocket.com/perftest/uploadtest/uploadstatus.asp");

			var theExpectedFileSizeInBytes = theFile.Length;

			using (var response = Post(uri, new MultipartFormData(), payload)) {
				var body = Body(response);

				var expectedMessage = String.Format("You uploaded {0} bytes", theExpectedFileSizeInBytes);

				Assert.That(body, Is.StringContaining(expectedMessage));
			}
		}
	}
}

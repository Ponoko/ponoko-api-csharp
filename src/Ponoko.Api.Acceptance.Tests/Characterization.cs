using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;

namespace Ponoko.Api.Acceptance.Tests {
	[TestFixture]
	public class Characterization : AcceptanceTest {
		[Test, Ignore("Probably not required now, but useful as verification.")]
		public void oauth_works_on_example_site() {
			var credentials = new CredentialSet(new Credential(
				ConfigurationManager.AppSettings["SevenDigital.Consumer.Key"],
				ConfigurationManager.AppSettings["SevenDigital.Consumer.Secret"]
			));

			if (String.IsNullOrEmpty(credentials.Consumer.Key))
				Assert.Fail(
					"This test requires valid consumer for 7digital API. " +
					"Ensure you have the expected settings in your configuration file."
				);

			var uri = new Uri("https://api.7digital.com/1.2/oauth/requesttoken");

			var parameters = new NameValueCollection(1) { { "q", "Phil Murphy is a plonker" } };

			using (var response = Get(uri)) {
				Assert.AreEqual(
					HttpStatusCode.OK, response.StatusCode,
					"Expected OK because OAuth is known to work"
				);
			}
		}

		[Test]
		public void can_post_a_file_and_it_emerges_with_correct_size() {
			var parameters = new NameValueCollection {
			    {"SUBMIT", "Upload!"}, 
			    {"xxx", "xxx"}, 
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

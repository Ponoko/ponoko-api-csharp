using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Unit.Tests.Rest.Mime {
	[TestFixture]
	public class FormUrlEncodedTests {
		[Test]
		public void it_returns_empty_payload_produces_empty_result() {
			var instance = new FormUrlEncoded();
			using (var result = instance.Format(Payload.Empty)) {
				Assert.AreEqual(0, result.ContentLength, "Expected zero content length for empty payload");
				Assert.AreEqual(0, result.Open().Length, "Expected zero bytes for empty payload");
			}
		}

		[Test]
		public void it_returns_a_non_zero_content_length_for_more_than_zero_fields() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { {"name", "value"} };

			using (var result = instance.Format(payload)) {
				Assert.That(result.ContentLength, Is.GreaterThan(0), "Expected zero content length for empty payload");
			}
		}

		[Test]
		public void the_content_length_returned_matches_the_length_of_the_stream() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { "name", "value" } };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual(result.Open().Length, result.ContentLength, 
					"Expected the value returned by the ContentLength property to match " + 
					"the number of bytes contained by the underlying stream"
				);
			}
		}

		[Test]
		public void it_returns_a_single_field() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { "name", "value" } };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual("name=value", ToText(result), "Expected the fi8eld to have been written");
			}
		}

		[Test]
		public void it_returns_multiple_fields_joining_them_with_ampersands() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { "name", "value" }, { "name_1", "value_1"} };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual("name=value&name_1=value_1", ToText(result), 
					"Expected both  of the fields to have been written, and to have been joined with ampersand"
				);
			}
		}

		[Test]
		public void it_url_encodes_each_field() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { "Full name", "Phil Murphy" } };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual("Full%20name=Phil%20Murphy", ToText(result), "Expected the fi8eld to have been written");
			}
		}

		[Test]
		public void fields_with_no_value_are_ignored_for_better_or_worse() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { "name", null }, { "name_1", "value_1"} };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual("name_1=value_1", ToText(result), "Expected the field to have been skipped");
			}
		}

		[Test]
		public void fields_with_no_name_are_ignored() {
			var instance = new FormUrlEncoded();
			var payload = new Payload { { null, "value" }, { "name_1", "value_1"} };

			using (var result = instance.Format(payload)) {
				Assert.AreEqual("name_1=value_1", ToText(result), "Expected the field to have been skipped");
			}
		}

		private String ToText(Body result) {
			using (var rdr = new StreamReader(result.Open())) {
				return rdr.ReadToEnd();	
			}
		}
	}
}

using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class ErrorDeserializerTests {
		[Test]
		public void can_deserialize_an_error_ignoring_request() {
			var json = "{" + 
				"'error': {" + 
				"	'message':'Bad Request. Error processing design file(s).'," + 
				"	'errors':[" +
				"		{" + 
				"			'type':'design_processing'," + 
				"			'node_key':'any_node_key'," +
				"			'error_code':'unknown_material'," + 
				"			'error_message':'any error message'," + 
				"			'name':'bottom_new.stl'," + 
				"			'material_key':'any_material_key'" +
				"		}" + 
				"	]," + 
				"	'request':{'name':'xxx','key':null,'designs':[{'quantity':'0','uploaded_data':'/tmp/RackMultipart25372-1','filename':'res\\bottom_new.stl','material_key':'','ref':''}]}" + 
				"}" + 
			"}";

			var result = ErrorDeserializer.Deserialize(json);

			Assert.AreEqual("Bad Request. Error processing design file(s).", result.Message);
			Assert.AreEqual(1, result.Errors.Count, "Expected a single error");
			
			var theError = result.Errors[0];

			Assert.AreEqual("design_processing"	, theError.Type, "Unexpected error type");
			Assert.AreEqual("any_node_key"		, theError.NodeKey, "Unexpected node key");
			Assert.AreEqual("unknown_material"	, theError.ErrorCode, "Unexpected error code");
			Assert.AreEqual("any error message"	, theError.Value, "Unexpected error message");
			Assert.AreEqual("bottom_new.stl"	, theError.Name, "Unexpected name");
			Assert.AreEqual("any_material_key"	, theError.MaterialKey, "Unexpected material key");
		}

		[Test] 
		public void can_deserialize_an_error_with_no_errors_array() {
			var json = "{" + 
				"'error': {" + 
				"	'message':'Bad Request. Error processing design file(s).'," + 
				"	'errors': null," + 
				"	'request':{'name':'xxx','key':null,'designs':[{'quantity':'0','uploaded_data':'/tmp/RackMultipart25372-1','filename':'res\\bottom_new.stl','material_key':'','ref':''}]}}" + 
				"}";

			var result = ErrorDeserializer.Deserialize(json);

			Assert.IsNull(result.Errors, "Expected the errors list to be set to null when the json has null as its errors array.");
		}

		[Test]
		public void can_convert_to_string_with_no_errors_list() {
			var error = new Error { Message = "This has no errors list"};
			var result = error.ToString();
			Assert.That(result, Is.StringMatching("This has no errors list"));
		}
	}
}
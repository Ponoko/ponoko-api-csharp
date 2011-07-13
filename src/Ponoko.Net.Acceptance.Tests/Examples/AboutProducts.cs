﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Net.Json;
using Ponoko.Net.Rest;

namespace Ponoko.Net.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutProducts : AcceptanceTest {
    	[Test]
		public void can_create_a_product_provided_you_have_a_design_file_and_a_matching_valid_material() {
			var parameters = new NameValueCollection {
				{"name"						, "example"}, 
				{"designs[][ref]"			, "42"},
				{"designs[][filename]"		, "bottom_new.stl"},
				{"designs[][quantity]"		, "1"},
				{"designs[][material_key]"	, "6bb50fd03269012e3526404062cdb04a"},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(@"res\bottom_new.stl"), "text/plain"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile), Credentials)) {
				var body = Json(response);
				Console.WriteLine(body);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, body);
			}
    	}

		[Test]
    	public void you_must_supply_a_design_when_adding_a_product() {
    		var parameters = new NameValueCollection {
				{"name", "example"}, 
				{"notes", "This one is supposed to fail because it has missing design"}
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters), Credentials)) {
				var body = Json(response);

				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, body);

				Assert.That(body, Is.StringMatching("\"message\": \"Bad Request. Product must have a design.\""));
			}
		}

		[Test]
    	public void you_must_supply_a_file_and_filename() {
    		var parameters = new NameValueCollection {
				{"name"						, "example"}, 
				{"designs[][ref]"			, "42"},
				{"designs[][quantity]"		, "1"},
				{"designs[][material_key]"	, "6bb50fd03269012e3526404062cdb04a"},
			};

			var missingFile = new List<DataItem> ();

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, missingFile), Credentials)) {
				var body = Json(response);
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, body);
				Assert.That(body, Is.StringMatching("\"error_message\": \"Missing design file data.\""));
			}
		}

		[Test]
    	public void you_must_supply_a_material() {
    		var parameters = new NameValueCollection {
				{"name"						, "example"}, 
				{"designs[][ref]"			, "42"},
				{"designs[][filename]"		, "bottom_new.stl"},
				{"designs[][quantity]"		, "1"},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(@"res\bottom_new.stl"), 
					"text/plain"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile), Credentials)) {
				var body = Json(response);
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, body);
				Assert.That(body, Is.StringMatching("could not find requested material. is it available to this Node's materail catalog?"));
			}
		}

		[Test]
		public void and_the_material_must_be_compatible() {
			const String FUCHSIA_FELT = "6812d5403269012e2f2f404062cdb04a";

			var parameters = new NameValueCollection {
				{"name"						, "example"}, 
				{"designs[][ref]"			, "42"},
				{"designs[][filename]"		, "bottom_new.stl"},
				{"designs[][quantity]"		, "1"},
				{"designs[][material_key]"	, FUCHSIA_FELT},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(@"res\bottom_new.stl"), "text/plain"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile), Credentials)) {
				var body = Json(response);
				Console.WriteLine(body);
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, body);
				Assert.That(body, Is.StringMatching("the material you have selected is not compatible with making methods available to the design file"));
			}
		}

		[Test]
		public void can_get_a_list_of_products() {
			given_at_least_one_product();

			var uri = Map("/products");

			using (var response = Get(uri, Credentials)) {
				var result = new Deserializer().Deserialize(Body(response));
				
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected okay");

				var products = result["products"].First.Value<String>("key");

				Console.WriteLine(products);
			}
		}

		[Test]
		public void can_get_a_single_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			var uri = Map("/products/{0}", id);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Console.WriteLine(Json(response));
			}
		}

		[Test]
		public void you_can_check_existence_of_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			var uri = Map("/products/{0}", id);

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			}

			uri = Map("/products/{0}", "MUST_NOT_EXIST");

			using (var response = Get(uri, Credentials)) {
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[Test]
		public void can_delete_a_product() {
			given_at_least_one_product();
			
			var id = FindFirstProductKey();
			var uri = Map("/products/delete/{0}", id);

			using (var response = Get(Map("/products/{0}", id), Credentials)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
					"Expected the product to exist before we delete it."
				);
			}

			var theStatusReturnedByDelete = -1;

			using (var response = Post(uri, Payload.Empty, Credentials)) {
				theStatusReturnedByDelete = (Int32)response.StatusCode;
			}

			using (var response = Get(Map("/products/{0}", id), Credentials)) {
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, 
					"Expected the product to have been deleted, but it's still there."
				);
			}

			Assert.AreEqual(200, theStatusReturnedByDelete, "Expected delete to return status 200.");
		}

		private void given_at_least_one_product() {
    		var parameters = new NameValueCollection {
				{"name"						, "example"}, 
				{"designs[][ref]"			, "1337"},
				{"designs[][filename]"		, "bottom_new.stl"},
				{"designs[][quantity]"		, "1"},
				{"designs[][material_key]"	, "6bb50fd03269012e3526404062cdb04a"},
			};

			var theFile = new List<DataItem> {
				new DataItem(
					"designs[][uploaded_data]", 
					new FileInfo(@"res\bottom_new.stl"), "text/plain"
				)
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters, theFile), Credentials)) {
				var body = Body(response);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, body);
			}
    	}

		private String FindFirstProductKey() {
			var uri = Map("/products");

			using (var response = Get(uri, Credentials)) {
				var temp = new Deserializer().Deserialize(Body(response));
				
				var products = temp["products"];

				Assert.That(products.HasValues, "Ther are zero products, so unable to return the first one.");
				
				return products.First.Value<String>("key");
			}
		}
	}
}
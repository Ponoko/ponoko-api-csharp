﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutProducts : AcceptanceTest {
    	[Test]
		public void can_create_a_product_provided_you_have_a_design_file_with_matching_valid_material() {
			var authorizationPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
				Settings.Credentials
			);
			
			var theInternet = new SystemInternet(authorizationPolicy);

    		var products = new Products(theInternet, Settings.BaseUrl);

    		var expectedDesign = NewDesign();

    		var expectedNewProductName = "Any new product name";

    		var theNewProduct = products.Save(expectedNewProductName, expectedDesign);
    		var actualDesign = theNewProduct.Designs[0];

			Assert.AreEqual(expectedNewProductName	, theNewProduct.Name, "Expected the returned product to have the name supplied.");
			Assert.IsFalse(theNewProduct.IsLocked	, "Expected the newly-created product to be unlocked.");
			Assert.IsNotNull(theNewProduct.Designs	, "Expected the product to be returned with a key.");
    		Assert.IsNotNull(theNewProduct.NodeKey	, "Expected the product to be returned with a node key.");

    		AssertIsAboutUtcNow(theNewProduct.CreatedAt, TimeSpan.FromSeconds(5));
    		AssertIsAboutUtcNow(theNewProduct.UpdatedAt, TimeSpan.FromSeconds(5));
    		
			Assert.AreEqual(1, theNewProduct.Designs.Count, "Expected the new product to have the design we supplied.");
    		AssertEqual(expectedDesign, actualDesign);
			Assert.That(actualDesign.MaterialKey, Is.Null, "Expectecd no material key because the product's materials are not available.");
    	}

		private void AssertIsAboutUtcNow(DateTime dateTime, TimeSpan within) {
			var now = DateTime.UtcNow;
			var diff = now.Subtract(dateTime);
			Assert.That(diff, Is.LessThan(within));
		}

		// TEST: if_a_design_does_not_have_material_available_then_it_does_not_have_material_key

		private void AssertEqual(Design expected, Design actual) {
			Assert.AreEqual(Path.GetFileName(expected.Filename), actual.Filename);
			Assert.AreEqual(expected.Quantity, actual.Quantity);
			Assert.AreEqual(expected.Reference, actual.Reference);
			
			Assert.AreEqual(actual.MakeCost.Total		, 0D);
			Assert.AreEqual(actual.MakeCost.Making		, 0D);
			Assert.AreEqual(actual.MakeCost.Materials	, 0D);
			Assert.AreEqual(actual.MakeCost.Currency	, "USD");
		}

		[Test]
    	public void you_must_supply_a_design_when_adding_a_product() {
    		var parameters = new NameValueCollection {
				{"name", "example"}, 
				{"notes", "This one is supposed to fail because it has missing design"}
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new Payload(parameters))) {
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

			using (var response = Post(uri, new Payload(parameters, missingFile))) {
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

			using (var response = Post(uri, new Payload(parameters, theFile))) {
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

			using (var response = Post(uri, new Payload(parameters, theFile))) {
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

			using (var response = Get(uri)) {
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

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Console.WriteLine(Json(response));
			}
		}

		[Test]
		public void you_can_check_existence_of_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			var uri = Map("/products/{0}", id);

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			}

			uri = Map("/products/{0}", "MUST_NOT_EXIST");

			using (var response = Get(uri)) {
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[Test]
		public void can_delete_a_product() {
			given_at_least_one_product();
			
			var id = FindFirstProductKey();
			var uri = Map("/products/delete/{0}", id);

			using (var response = Get(Map("/products/{0}", id))) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, 
					"Expected the product to exist before we delete it."
				);
			}

			var theStatusReturnedByDelete = -1;

			using (var response = Post(uri, Payload.Empty)) {
				theStatusReturnedByDelete = (Int32)response.StatusCode;
			}

			using (var response = Get(Map("/products/{0}", id))) {
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

			using (var response = Post(uri, new Payload(parameters, theFile))) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, Body(response));
			}
    	}

		private Design NewDesign() {
			var aValidMaterialKey = "6bb50fd03269012e3526404062cdb04a";

			return new Design {
				Filename	= new FileInfo(@"res\bottom_new.stl").FullName,
				MaterialKey = aValidMaterialKey,
				Quantity	= 1,
				Reference	= "42"
			};
		}

		private String FindFirstProductKey() {
			var uri = Map("/products");

			using (var response = Get(uri)) {
				var temp = new Deserializer().Deserialize(Body(response));
				
				var products = temp["products"];

				Assert.That(products.HasValues, "Ther are zero products, so unable to return the first one.");
				
				return products.First.Value<String>("key");
			}
		}
	}
}

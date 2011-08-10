using System;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public static class ExampleMaterials {
		public const String METAL = "6b0fa5b03269012e3438404062cdb04a";
		public const String ACRYLIC = "68876ab03269012e2fd9404062cdb04a";
		public const String DURABLE_PLASTIC = "6bb50fd03269012e3526404062cdb04a";
		public const String SUPERFINE_PLASTIC = "6bb5ac203269012e3528404062cdb04a";	
	}

	public class ProductAcceptanceTest : AcceptanceTest {
		public CreateCommand CreateCommand { get; set; }

		[SetUp]
		public void BeforeEach() {
			CreateCommand = new CreateCommand(Internet, Settings.BaseUrl, new DefaultProductValidator());
		}

		protected void given_at_least_one_product() {
			given_at_least_one_product("example");
		}

		protected void given_at_least_one_product(String called) {
			NewProduct(called);
		}

		protected Product NewProduct(string called) {
			var payload = new Payload {
         		{ "name"					, called}, 
         		{ "designs[][ref]"			, "1337"},
         		{ "designs[][filename]"		, "bottom_new.stl"},
         		{ "designs[][quantity]"		, "1"},
         		{ "designs[][material_key]"	, ExampleMaterials.DURABLE_PLASTIC},
         		{ "file", new DataItem("designs[][uploaded_data]", new FileInfo(@"res\bottom_new.stl"), "text/plain")},
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new MultipartFormData(), payload)) {
				var text = Body(response);
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected the delete to return 200. The response returned: {0}", text);
				var json = TryDeserializeProduct(text);
				return ProductDeserializer.Deserialize(json);
			}
		}

		private String TryDeserializeProduct(String text) {
			try {
				return new Deserializer().Deserialize(text)["product"].ToString();
			} catch (Exception e) {
				throw new Exception(String.Format("Failed to deserialize: <{0}>", text));
			}
		}

		protected String FindFirstProductKey() {
			var uri = Map("/products");

			using (var response = Get(uri)) {
				var temp = new Deserializer().Deserialize(Body(response));
				
				var products = temp["products"];

				Assert.That(products.HasValues, "There are zero products, so unable to return the first one.");
				
				return products.First.Value<String>("key");
			}
		}

		protected void Delete(Product product) {
			Assert.Throws<Exception>(() => 
				new DeleteCommand(Internet, Settings.BaseUrl).Delete(product.Key), 
			    "Expected this to fail because at the time of writing the delete operation is broken on the server end. " + 
			    "It didn't fail, so the remote end has been fixed and you can remove this assertion."
			);
		}

		protected Design NewDesign() {
			const String VALID_MATERIAL_KEY = "6bb50fd03269012e3526404062cdb04a";

			return new Design {
              	Filename	= new FileInfo(@"res\bottom_new.stl").FullName,
              	MaterialKey = VALID_MATERIAL_KEY,
              	Quantity	= 1,
              	Reference	= "42"
			};
		}
	}
}
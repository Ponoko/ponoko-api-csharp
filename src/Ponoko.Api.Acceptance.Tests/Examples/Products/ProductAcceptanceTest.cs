using System;
using System.Collections.Generic;
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
			var parameters = new List<Field> {
         		new Field { Name = "name"						, Value = called}, 
         		new Field { Name = "designs[][ref]"				, Value = "1337"},
         		new Field { Name = "designs[][filename]"		, Value = "bottom_new.stl"},
         		new Field { Name = "designs[][quantity]"		, Value = "1"},
         		new Field { Name = "designs[][material_key]"	, Value = "6bb50fd03269012e3526404062cdb04a"},
         		new Field { 
					Name = "file", 
					Value = new DataItem(
						"designs[][uploaded_data]", 
						new FileInfo(@"res\bottom_new.stl"), "text/plain"
					)
				},
			};

			var uri = Map("{0}", "/products");

			using (var response = Post(uri, new MultipartFormData(), new Payload(parameters))) {
				var json = new Deserializer().Deserialize(Body(response))["product"].ToString();
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, json);
				return ProductDeserializer.Deserialize(json);
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
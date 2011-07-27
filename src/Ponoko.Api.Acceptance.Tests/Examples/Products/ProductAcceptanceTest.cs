using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core.IO;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class ProductAcceptanceTest : AcceptanceTest {
		public CreateCommand CreateCommand { get; set; }

		[SetUp]
		public void BeforeEach() {
			CreateCommand = new CreateCommand(Internet, Settings.BaseUrl, new DefaultProductValidator(new DefaultReadonlyFileSystem()));
		}

		protected void given_at_least_one_product() {
			given_at_least_one_product("example");
		}

		protected void given_at_least_one_product(String called) {
			var parameters = new List<Parameter> {
             	new Parameter { Name = "name" , Value = called}, 
             	new Parameter { Name = "designs[][ref]"			, Value = "1337"},
             	new Parameter { Name = "designs[][filename]"		, Value = "bottom_new.stl"},
             	new Parameter { Name = "designs[][quantity]"		, Value = "1"},
             	new Parameter { Name = "designs[][material_key]"	, Value = "6bb50fd03269012e3526404062cdb04a"},
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

		protected String FindFirstProductKey() {
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
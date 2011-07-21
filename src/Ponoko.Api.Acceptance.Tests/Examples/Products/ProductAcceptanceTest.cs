using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core.IO;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class ProductAcceptanceTest : AcceptanceTest {
		public Core.Products Products { get; set; }

		[SetUp]
		public void BeforeEach() {
			Products = new Core.Products(NewInternet(), Settings.BaseUrl, new DefaultReadonlyFileSystem());
		}

		protected void given_at_least_one_product() {
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

		protected String FindFirstProductKey() {
			var uri = Map("/products");

			using (var response = Get(uri)) {
				var temp = new Deserializer().Deserialize(Body(response));
				
				var products = temp["products"];

				Assert.That(products.HasValues, "Ther are zero products, so unable to return the first one.");
				
				return products.First.Value<String>("key");
			}
		}

		private SystemInternet NewInternet() {
			var authorizationPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
				Settings.Credentials
			);

			return new SystemInternet(authorizationPolicy);
		}
	}
}
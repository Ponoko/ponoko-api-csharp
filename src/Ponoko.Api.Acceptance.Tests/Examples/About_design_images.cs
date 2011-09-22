using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Acceptance.Tests.Examples.Products;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class About_design_images : ProductAcceptanceTest {
		[Test]
		public void you_can_add_a_design_image_to_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var theProduct = new DesignImageAddCommand(Internet, Settings.BaseUrl).Add(id, theImage);

			Assert.That(theProduct.DesignImages.Count, Is.GreaterThan(0), "Expected at least one design image");

			Assert.That(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name))
			);
		}

		[Test, Ignore("PENDING")]
		public void you_cannot_add_a_design_image_if_the_product_does_not_exist() { }

		[Test, Ignore("PENDING")]
		public void you_can_get_the_design_images_for_a_product() { }

		[Test, Ignore("PENDING")]
		public void you_may_get_an_auto_generated_image() { }
	}

	public class DesignImageAddCommand : Domain {
		public DesignImageAddCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) { }

		public Product Add(String product, FileInfo file) {
			var uri = Map("/products/{0}/design-images", product);

			// TODO: why do I need to supply the field name twice?
			var payload = new Payload{{
				"design_images[][uploaded_data]", 
				new DataItem("design_images[][uploaded_data]", file, "image/gif")
			}};

			using (var response = MultipartPost(uri, payload)) {
				Console.WriteLine(response.StatusCode);

				var json = ReadAll(response);

				var productJson = new Deserializer().Deserialize(json)["product"];
				return ProductDeserializer.Deserialize(productJson.ToString());
			}
		}
	}
}

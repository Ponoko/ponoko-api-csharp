using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Acceptance.Tests.Examples.Products;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class About_design_images : ProductAcceptanceTest {
		[Test]
		public void you_can_add_a_design_image_to_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();
			
			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			new DesignImageCommand(Internet, Settings.BaseUrl).Add(id, theImage);

			var theProduct = new FindCommand(Internet, Settings.BaseUrl).Find(id);
			
			Assert.That(theProduct.DesignImages.Count, Is.GreaterThan(0), "Expected at least one design image");
			Assert.That(theProduct.DesignImages.Exists(di => di.Filename == Path.GetFileName(theImage.Name)));
		}

		[Test]
		public void you_can_get_the_design_images_for_a_product() {}
	}

	public class DesignImageCommand : Domain {
		public DesignImageCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}
		
		public void Add(String product, FileInfo file) {
			var uri = Map("/products/{0}/design-images", product);

			// TODO: why do I need to supply the field name twice?
			var payload = new Payload{{
				"design_images[][uploaded_data]", 
				new DataItem("design_images[][uploaded_data]", file, "image/gif")
			}};

			using (var response = MultipartPost(uri, payload)) {
				Console.WriteLine(response.StatusCode);	
				Console.WriteLine(ReadAll(response));	
			}
		}
	}
}

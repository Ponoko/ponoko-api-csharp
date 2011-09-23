using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

		[Test]
		public void you_can_get_a_design_image_for_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var command = new DesignImageAddCommand(Internet, Settings.BaseUrl);

			command.Add(id, theImage);

			var result = command.Get(id, theImage.Name);

			Assert.That(result.Length, Is.EqualTo(theImage.Length),
				"Expected the returned file to have exactly the same size as the one we uploaded"
			);

			var expectedChecksum = Checksum(File.ReadAllBytes(theImage.FullName));
			var actualChecksum = Checksum(result);

			Assert.AreEqual(expectedChecksum, actualChecksum, "The file returned is not identical");
		}
		
		private String Checksum(Byte[] file) {
			MD5 md5 = new MD5CryptoServiceProvider();
			var checksum = md5.ComputeHash(file);
			var buffer = new StringBuilder();

			for (var i = 0; i < checksum.Length; i++) {
				buffer.Append(checksum[i].ToString("x2"));
			}

			return buffer.ToString();
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

			var payload = new Payload { { "design_images[][uploaded_data]", new DataItem(file, "image/gif") } };

			using (var response = MultipartPost(uri, payload)) {
				var json = ReadAll(response);

				var productJson = new Deserializer().Deserialize(json)["product"];
				return ProductDeserializer.Deserialize(productJson.ToString());
			}
		}

		public Byte[] Get(String product, String filename) {
			var uri = Map("/products/{0}/design-images/download?filename={1}", product, filename);

			using (var response = Get(uri)) {
				var length = Int32.Parse(response.Header("Content-length"));
				return ReadAll(response.Open(), length);
			}
		}

		private byte[] ReadAll(Stream input, Int32 length) {
			const Int32 BUFFER_SIZE = 1024 * 10;

			using (var output = new MemoryStream(length)) {
				var buffer = new Byte[BUFFER_SIZE];
				var bytesRead = 0;

				while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
					output.Write(buffer, 0, bytesRead);
				}

				return output.GetBuffer();
			}
		}
	}
}

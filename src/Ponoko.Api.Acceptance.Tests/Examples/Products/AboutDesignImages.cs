using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using DesignImage = Ponoko.Api.Core.Product.Commands.DesignImage;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDesignImages : ProductAcceptanceTest {
		private Product AnyProduct;
		private DesignImages _designImages;

		[TearDown]
		public void AfterEach() {
			new DeleteCommand(Internet, Settings.BaseUrl).Delete(AnyProduct.Key);
		}

		[SetUp]
		new public void BeforeEach() {
			AnyProduct = NewProduct("Example for testing design images");
		}

		private DesignImages DesignImages {
			get { return _designImages ?? (_designImages = new DesignImages(Internet, Settings.BaseUrl)); }
		}

		[Test]
		public void you_can_add_a_design_image_to_a_product() {
			var theImage = new DesignImage(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = DesignImages.Add(AnyProduct.Key, theImage);

			Assert.That(theProduct.DesignImages.Count, Is.GreaterThan(0), "Expected at least one design image");

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.FileInfo.Name)), 
				"The design image <{0}> was not added", theImage.FileInfo.Name
			);
		}

		[Test]
		public void you_can_add_multiple_design_images_to_a_product() {
			var theImage = new DesignImage(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");
			var anotherImage = new DesignImage(new FileInfo("res\\example image with spaces.gif"), "image/gif");
			
			var theProduct = DesignImages.Add(AnyProduct.Key, theImage, anotherImage);

			Assert.That(theProduct.DesignImages.Count, Is.GreaterThan(0), "Expected at least one design image");

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.FileInfo.Name)), 
				"The design image <{0}> was not added", theImage.FileInfo.Name
			);

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName("example_image_with_spaces.gif")), 
				"The design image <example_image_with_spaces.gif> was not added"
			);
		}

		[Test] 
		public void you_get_an_error_if_you_supply_incorrect_content_type() {
			var theImage = new DesignImage(new FileInfo("res\\ponoko_logo_text_page.gif"), "xxx_clearly_invalid_content_type_xxx");

			var theError = Assert.Throws<Exception>(() => 
				DesignImages.Add(AnyProduct.Key, theImage)
			);

			Assert.That(theError.Message, Is.StringEnding("\"Bad Request. Error adding image\""), 
				"Unexpected error message returned"
			);
		}

		[Test]
		public void when_you_add_file_with_a_name_containing_spaces_they_are_replaced_with_underscores() {
			var theImage = new DesignImage(new FileInfo("res\\example image with spaces.gif"), "image/gif");

			var theProduct = DesignImages.Add(AnyProduct.Key, theImage);
			
			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName("example_image_with_spaces.gif")), 
				"The design image <example_image_with_spaces.gif> was not added"
			);
		}

		[Test]
		public void you_can_get_a_design_image_for_a_product() {
			var theImage = new DesignImage(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			DesignImages.Add(AnyProduct.Key, theImage);

			var result = ReadAll(DesignImages.Get(AnyProduct.Key, theImage.FileInfo.Name));

			Assert.AreEqual(theImage.FileInfo.Length, result.Length,
				"Expected the returned file to have exactly the same size as the one we uploaded"
			);

			var expectedChecksum = Checksum(File.ReadAllBytes(theImage.FileInfo.FullName));
			var actualChecksum = Checksum(result);

			Assert.AreEqual(expectedChecksum, actualChecksum, 
				"Expected the file returned to be identical to the one uploaded"
			);
		}

		[Test]
		public void you_can_remove_a_design_image_from_a_product() {
			var theImage = new DesignImage(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = DesignImages.Add(AnyProduct.Key, theImage);

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.FileInfo.Name)),
				"The design image was not added"
			);

			theProduct = DesignImages.Remove(AnyProduct.Key, theImage.FileInfo.Name);

			Assert.IsFalse(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.FileInfo.Name)), 
				"Expected the design image to hav been deleted"
			);
		}

		[Test, Ignore("PENDING")]
		public void you_cannot_add_a_design_image_if_the_product_does_not_exist() { }

		[Test, Ignore("PENDING")]
		public void you_may_get_an_auto_generated_image() { }

		private String Checksum(Byte[] file) {
			var checksum = new MD5CryptoServiceProvider().ComputeHash(file);
			var buffer = new StringBuilder();

			for (var i = 0; i < checksum.Length; i++) {
				buffer.Append(checksum[i].ToString("x2"));
			}

			return buffer.ToString();
		}

		private Byte[] ReadAll(Stream input) {
			const Int32 BUFFER_SIZE = 1024 * 10;

			using (var output = new MemoryStream()) {
				var buffer = new Byte[BUFFER_SIZE];
				var bytesRead = 0;
				var totalBytesRead = 0;
				while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
					output.Write(buffer, 0, bytesRead);
					totalBytesRead += bytesRead;
				}

				var result = new Byte[totalBytesRead];

				Buffer.BlockCopy(output.GetBuffer(), 0, result, 0, totalBytesRead);

				return result;
			}
		}
	}
}

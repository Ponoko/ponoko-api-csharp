using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Acceptance.Tests.Examples.Products;
using Ponoko.Api.Core.Product.Commands;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutDesignImages : ProductAcceptanceTest {
		// TODO: Are these methods poorly-hung? Passing Product seems to smell somehow.
		[Test]
		public void you_can_add_a_design_image_to_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var theProduct = new AddDesignImageCommand(Internet, Settings.BaseUrl).Add(id, theImage);

			Assert.That(theProduct.DesignImages.Count, Is.GreaterThan(0), "Expected at least one design image");

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name)), 
				"The design image was not added"
			);
		}

		[Test]
		public void when_you_add_file_with_a_name_containing_spaces_they_are_replaced_with_underscores() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\example image with spaces.gif");

			var theProduct = new AddDesignImageCommand(Internet, Settings.BaseUrl).Add(id, theImage);
			
			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName("example_image_with_spaces.gif")), 
				"The design image was not added"
			);
		}

		[Test]
		public void you_can_get_a_design_image_for_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var command = new AddDesignImageCommand(Internet, Settings.BaseUrl);

			command.Add(id, theImage);

			var result = ReadAll(command.Get(id, theImage.Name));

			Assert.AreEqual(theImage.Length, result.Length,
				"Expected the returned file to have exactly the same size as the one we uploaded"
			);

			var expectedChecksum = Checksum(File.ReadAllBytes(theImage.FullName));
			var actualChecksum = Checksum(result);

			Assert.AreEqual(expectedChecksum, actualChecksum, 
				"Expected the file returned to be identical to the one uploaded"
			);
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

		[Test]
		public void you_can_remove_a_design_image_from_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var command = new AddDesignImageCommand(Internet, Settings.BaseUrl);
			var theProduct = command.Add(id, theImage);

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name)),
				"The design image was not added"
			);

			theProduct = command.Remove(id, theImage.Name);

			Assert.IsFalse(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name)), 
				"Expected the design image to hav been deleted"
			);
		}

		private String Checksum(Byte[] file) {
			var checksum = new MD5CryptoServiceProvider().ComputeHash(file);
			var buffer = new StringBuilder();

			for (var i = 0; i < checksum.Length; i++) {
				buffer.Append(checksum[i].ToString("x2"));
			}

			return buffer.ToString();
		}

		[Test, Ignore("PENDING")]
		public void you_cannot_add_a_design_image_if_the_product_does_not_exist() { }

		[Test, Ignore("PENDING")]
		public void you_may_get_an_auto_generated_image() { }
	}
}

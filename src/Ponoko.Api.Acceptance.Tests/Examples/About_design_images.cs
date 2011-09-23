﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Acceptance.Tests.Examples.Products;
using Ponoko.Api.Core.Product.Commands;

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
		
		[Test]
		public void you_can_remove_a_design_image_from_a_product() {
			given_at_least_one_product();

			var id = FindFirstProductKey();

			var theImage = new FileInfo("res\\ponoko_logo_text_page.gif");

			var command = new DesignImageAddCommand(Internet, Settings.BaseUrl);
			var theProduct = command.Add(id, theImage);

			Assert.IsTrue(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name))
			);

			theProduct = command.Remove(id, theImage.Name);

			Assert.IsFalse(theProduct.DesignImages.Exists(it =>
				it.Filename == Path.GetFileName(theImage.Name))
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

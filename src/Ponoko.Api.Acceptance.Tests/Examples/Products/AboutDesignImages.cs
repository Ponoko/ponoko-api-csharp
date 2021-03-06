﻿using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Repositories;
using File = Ponoko.Api.Core.Product.File;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutDesignImages : ProductFileRepositoryAcceptanceTest {
		private Product AnyProduct;
		private DesignImageRepository DesignImageRepository;

		[TestFixtureSetUp]
		public void TestFixtureSetUp() {
			DesignImageRepository = new DesignImageRepository(Internet, Settings.BaseUrl);
			AnyProduct = NewProduct("Example for testing design images");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() {
			Delete(AnyProduct);
		}

		[Test]
		public void you_can_add_a_design_image_to_a_product() {
			var theImage = NewRandomFile();

			var theProduct = DesignImageRepository.Add(AnyProduct.Key, theImage);

			AssertIncludesDesignImage(theProduct, theImage);
		}

		[Test]
		public void you_can_add_multiple_design_images_to_a_product() {
			var theImage = NewRandomFile();
			var anotherImage = NewRandomFile();
			
			var theProduct = DesignImageRepository.Add(AnyProduct.Key, theImage, anotherImage);

			AssertIncludesDesignImage(theProduct, theImage);

			Assert.IsTrue(theProduct.DesignImages.Exists(it => it.Filename == "example_image_with_spaces.gif"), 
				"The design image <example_image_with_spaces.gif> was not added"
			);
		}

		[Test] 
		public void you_get_an_error_if_you_supply_incorrect_content_type() {
			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "xxx_clearly_invalid_content_type_xxx");

			var theError = Assert.Throws<Exception>(() => 
				DesignImageRepository.Add(AnyProduct.Key, theImage), 
                "Expected an exception to be thrown because we supplied an invalid content type"
			);

			Assert.That(theError.Message, Is.StringEnding("\"Bad Request. Error adding image\""), 
				"Unexpected error message returned"
			);
		}

		[Test] 
		public void you_do_not_get_an_error_if_you_supply_a_content_type_that_does_not_match_the_file() {
			var fileInfo = new FileInfo("res\\ponoko_logo_text_page.gif");

			var theImage = new File(fileInfo, "image/png");
			var theProduct = DesignImageRepository.Add(AnyProduct.Key, theImage);
			AssertIncludesDesignImage(theProduct, theImage);
		}

		[Test]
		public void when_you_add_file_with_a_name_containing_spaces_they_are_replaced_with_underscores() {
			var theImage = new File(new FileInfo("res\\example image with spaces.gif"), "image/gif");

			var theProduct = DesignImageRepository.Add(AnyProduct.Key, theImage);
			
			Assert.IsTrue(theProduct.DesignImages.Exists(it => it.Filename == "example_image_with_spaces.gif"), 
				"The design image <example_image_with_spaces.gif> was not added"
			);
		}

		[Test, Ignore("PENDING")]
		public void file_names_are_also_lower_cased() { }

		[Test]
		public void you_can_get_a_design_image_for_a_product() {
			var theImage = NewRandomFile();
			var theFileOnDisk = new FileInfo(theImage.FullName);

			DesignImageRepository.Add(AnyProduct.Key, theImage);

			var result = ReadAll(DesignImageRepository.Get(AnyProduct.Key, theImage.Filename));

			Assert.AreEqual(theFileOnDisk.Length, result.Length,
				"Expected the returned file to have exactly the same size as the one we uploaded"
			);

			var expectedChecksum = Checksum(System.IO.File.ReadAllBytes(theFileOnDisk.FullName));
			var actualChecksum = Checksum(result);

			Assert.AreEqual(expectedChecksum, actualChecksum, 
				"Expected the file returned to be identical to the one uploaded"
			);
		}

		[Test]
		public void you_can_remove_a_design_image_from_a_product() {
			var theImage = NewRandomFile();

			var theProduct = DesignImageRepository.Add(AnyProduct.Key, theImage);

			Assert.IsTrue(theProduct.DesignImages.Exists(it => it.Filename == theImage.Filename),
				"The design image <{0}> was not added", theImage.FullName
			);

			theProduct = DesignImageRepository.Remove(AnyProduct.Key, theImage.Filename);

			Assert.IsFalse(theProduct.DesignImages.Exists(it => it.Filename == theImage.Filename), 
				"Expected the design image <{0}> to have been deleted, but it's still there", theImage.Filename
			);
		}

		private File NewRandomFile() {
			var newTempFile = new FileInfo(Path.GetTempFileName());
			newTempFile = new FileInfo(newTempFile.FullName.ToLower());

			System.IO.File.Copy("res\\ponoko_logo_text_page.gif", newTempFile.FullName.ToLower(), true);
			
			return new File(newTempFile, "image/gif");
		}

		[Test, Ignore("PENDING")]
		public void you_cannot_add_a_design_image_if_the_product_does_not_exist() { }

		[Test, Ignore("PENDING")]
		public void you_may_get_an_auto_generated_image() { }
	}
}

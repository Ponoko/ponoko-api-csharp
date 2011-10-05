using System;
using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Repositories;
using File = Ponoko.Api.Core.Product.File;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class AboutAssemblyInstructions : ProductFileRepositoryAcceptanceTest {
		private Product AnyProduct;
		private AssemblyInstructionRepository _assemblyInstructionRepository;

		public AssemblyInstructionRepository AssemblyInstructionRepository {
			get {
				return _assemblyInstructionRepository ??(
					_assemblyInstructionRepository = new AssemblyInstructionRepository(Internet, Settings.BaseUrl)
				);
			}
		}

		[TestFixtureSetUp]
		public void SetUp() {
			AnyProduct = NewProduct("Example for testing assembly instructions");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() { Delete(AnyProduct); }

		[Test]
		public void you_can_add_assembly_instructions_to_a_product() {
			RefreshProduct();

			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

			Assert.AreEqual(1, theProduct.AssemblyInstructions.Count, "Expected one assembly instructions file");
		}

		[Test]
		public void you_can_add_multiple_assembly_instructions_to_a_product() {
			RefreshProduct();

			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");
			var anotherImage = new File(new FileInfo("res\\example image with spaces.gif"), "image/gif");
			
			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage, anotherImage);

			Assert.AreEqual(2, theProduct.AssemblyInstructions.Count, "Expected two assembly instructions files.");

			Assert.IsTrue(theProduct.AssemblyInstructions.Exists(it => it.Filename == theImage.Filename), 
				"The assembly instructions file <example_image_with_spaces.gif> was not added"
			);

			Assert.IsTrue(theProduct.AssemblyInstructions.Exists(it => it.Filename == "example_image_with_spaces.gif"), 
				"The assembly instructions file <example_image_with_spaces.gif> was not added"
			);
		}

		[Test] 
		public void you_do_not_get_an_error_if_you_supply_incorrect_content_type() {
			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "xxx_clearly_invalid_content_type_xxx");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

			AssertIncludesAssemblyInstructions(theProduct, theImage);
		}

		[Test] 
		public void you_do_not_get_an_error_if_you_supply_a_content_type_that_does_not_match_the_file() {
			var fileInfo = new FileInfo("res\\ponoko_logo_text_page.gif");

			var theImage = new File(fileInfo, "image/png");
			var theProduct  = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);
			
			AssertIncludesAssemblyInstructions(theProduct, theImage);
		}

		[Test]
		public void when_you_add_file_with_a_name_containing_spaces_they_are_replaced_with_underscores() {
			var theImage = new File(new FileInfo("res\\example image with spaces.gif"), "image/gif");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);
			
			Assert.IsTrue(theProduct.AssemblyInstructions.Exists(it => it.Filename == "example_image_with_spaces.gif"), 
				"The design image <example_image_with_spaces.gif> was not added"
			);
		}

		[Test]
		public void you_can_get_an_assembly_instructions_file_for_a_product() {
		    var theFileOnDisk = new FileInfo("res\\ponoko_logo_text_page.gif");

		    var theImage = new File(theFileOnDisk, "image/gif");

		    AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

		    var result = ReadAll(AssemblyInstructionRepository.Get(AnyProduct.Key, theImage.Filename));

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
		public void you_can_remove_an_assembly_instructions_file_from_a_product() {
			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

			Assert.IsTrue(theProduct.AssemblyInstructions.Exists(it => it.Filename == theImage.Filename),
				"The assembly instructions file was not added"
			);

			theProduct = AssemblyInstructionRepository.Remove(AnyProduct.Key, theImage.Filename);

			Assert.IsFalse(theProduct.DesignImages.Exists(it => it.Filename == theImage.Filename), 
				"Expected the assembly instructions file <{0}> to have been deleted, but it's still there", theImage.Filename
			);
		}

		private void RefreshProduct() {
			Delete(AnyProduct);
			AnyProduct = NewProduct("Example for testing assembly instructions");
		}
	}
}

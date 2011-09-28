using System.IO;
using NUnit.Framework;
using Ponoko.Api.Core.Product;

namespace Ponoko.Api.Unit.Tests.Core.Product {
	[TestFixture]
	public class DesignImageTests {
		[Test]
		public void full_name_equals_filename_when_there_is_no_file_info() {
			var designImage = new DesignImage {Filename = "xxx"};
			Assert.AreEqual(designImage.Filename, designImage.FullName);
		}

		[Test]
		public void full_name_is_the_same_as_the_fileinfo_supplied_to_ctor() {
			var fileInfo = new FileInfo("D:\\path\\to\\xxx");
			var designImage = new DesignImage(fileInfo);
			Assert.AreEqual(fileInfo.FullName, designImage.FullName);
		}

		[Test]
		public void when_file_info_supplied_to_ctor_file_name_is_taken_from_that() {
			var fileInfo = new FileInfo("D:\\path\\to\\xxx");
			var designImage = new DesignImage(fileInfo);
			Assert.AreEqual(fileInfo.Name, designImage.Filename);
		}

		[Test]
		public void setting_file_name_clears_the_path_from_full_name_too() {
			var fileInfo = new FileInfo("D:\\path\\to\\xxx");
			var designImage = new DesignImage(fileInfo);

			designImage.Filename = "yyy_my_delilah";
			
			Assert.AreEqual(designImage.Filename, designImage.FullName);
		}
	}
}

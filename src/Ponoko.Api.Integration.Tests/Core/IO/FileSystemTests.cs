using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.IO;

namespace Ponoko.Api.Integration.Tests.Core.IO {
	[TestFixture]
	public class FileSystemTests {
		[Test]
		public void it_creates_a_file_on_disk() {
			var theFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			
			Assert.IsFalse(File.Exists(theFilePath), "Invalid test data, the file must not exist before it is created!");
			
			new DefaultFileSystem().New(theFilePath);

			Assert.IsTrue(File.Exists(theFilePath), "Expected the file to have been created on disk");
		}

		// TEST: new_files_are_writable_and_readable
	}

	public class DefaultFileSystem : FileSystem {
		public FileInfo New(string filename) {
			using(var newFile = File.Create(filename)) {}
			return new FileInfo(filename);
		}

		public FileStream Open(FileInfo filename) {
			throw new NotImplementedException();
		}

		public void Delete(FileInfo filename) {
			throw new NotImplementedException();
		}
	}
}

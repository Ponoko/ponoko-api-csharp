using System;
using System.IO;

namespace Ponoko.Api.Core.IO {
	public class DefaultFileSystem : FileSystem {
		public FileInfo New(String filename) {
			using (var newFile = File.Create(filename)) {}
			return new FileInfo(filename);
		}

		public FileStream Open(FileInfo filename) {
			return new FileStream(
				filename.FullName, 
				FileMode.Open, 
				FileAccess.ReadWrite, 
				FileShare.None, 
				1024
			);
		}

		public void Delete(FileInfo filename) {
			filename.Delete();
		}
	}
}
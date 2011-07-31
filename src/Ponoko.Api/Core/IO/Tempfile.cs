using System;
using System.IO;

namespace Ponoko.Api.Core.IO {
	public class Tempfile : IDisposable {
		private readonly FileSystem _fileSystem;
		private FileInfo File { get { return _file ?? (_file = NewFile()); } }
		private FileStream _out;
		private FileInfo _file;

		private FileStream Out {
			get { return _out ?? (_out = _fileSystem.Open(File)); }
		}

		public Tempfile(FileSystem fileSystem) {
			_fileSystem = fileSystem;
			_file = NewFile();
		}

		public void Write(Byte[] buffer, Int32 offset, Int32 count) {
			Out.Write(buffer, offset, count);
		}

		private FileInfo NewFile() {
			var tempDir = Path.GetTempPath();
			var randomFileName = Path.GetRandomFileName();
			var fullPath = Path.Combine(tempDir, randomFileName);

			return _fileSystem.New(fullPath);
		}

		public void Dispose() {
			_fileSystem.Delete(File);
		}
	}
}
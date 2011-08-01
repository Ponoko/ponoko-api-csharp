using System;
using System.IO;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.IO {
	public class TempFileStream : Stream {
		private readonly FileSystem _fileSystem;
		private FileInfo File { get { return _file ?? (_file = NewFile()); } }
		private FileStream _out;
		private FileInfo _file;

		private FileStream Out {
			get { return _out ?? (_out = _fileSystem.Open(File)); }
		}

		public TempFileStream(FileSystem fileSystem) {
			_fileSystem = fileSystem;
			_file = NewFile();
		}

		public override void Write(Byte[] buffer, Int32 offset, Int32 count) {
			Out.Write(buffer, offset, count);
		}
		
		public override void Close() {
			un.less(null == _out, () => _out.Close()); 
			_fileSystem.Delete(File);
		}

		public override Int64 Seek(Int64 offset, SeekOrigin origin) {
			return Out.Seek(offset, origin);
		}

		public override Int64 Position {
			get { return Out.Position; }
			set { Out.Position = value;  }
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return true; }
		}

		public override bool CanWrite {
			get { return true; }
		}

		public override void Flush() {
			Out.Flush();
		}

		public override Int32 Read(byte[] buffer, Int32 offset, Int32 count) {
			return Out.Read(buffer, offset, count);
		}

		public override void SetLength(Int64 value) {
			throw new NotImplementedException();
		}

		public override Int64 Length {
			get { throw new NotImplementedException(); }
		}

		private FileInfo NewFile() {
			var tempDir = Path.GetTempPath();
			var randomFileName = Path.GetRandomFileName();
			var fullPath = Path.Combine(tempDir, randomFileName);

			return _fileSystem.New(fullPath);
		}
	}
}
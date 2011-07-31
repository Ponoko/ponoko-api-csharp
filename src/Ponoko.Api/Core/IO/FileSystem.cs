using System;
using System.IO;

namespace Ponoko.Api.Core.IO {
	public interface FileSystem {
		FileInfo New(String filename);
		FileStream Open(FileInfo filename);
		void Delete(FileInfo filename);
	}
}
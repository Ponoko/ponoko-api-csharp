using System.IO;

namespace Ponoko.Api.Core.IO {
	public class DefaultReadonlyFileSystem : ReadonlyFileSystem {
		public bool Exists(string filename) { return File.Exists(filename); }
	}
}
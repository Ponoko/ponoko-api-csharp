using System;

namespace Ponoko.Api.Core.IO {
	public interface ReadonlyFileSystem {
		Boolean Exists(String filename);
	}
}
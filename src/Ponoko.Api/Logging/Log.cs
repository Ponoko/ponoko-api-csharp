using System;

namespace Ponoko.Api.Logging {
	public interface Log {
		void Info(String format, params Object[] args);
	}
}
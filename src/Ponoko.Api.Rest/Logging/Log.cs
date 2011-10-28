using System;

namespace Ponoko.Api.Rest.Logging {
	public interface Log {
		void Info(String format, params Object[] args);
	}
}
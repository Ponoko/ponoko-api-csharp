using System;

namespace Ponoko.Api.Rest {
	public interface HttpContentType : IDisposable {
		void WriteBody(IHttpRequest request, Payload payload);
		String ContentType { get; }
	}
}
using System;

namespace Ponoko.Net.Rest {
	public interface HttpContentType : IDisposable {
		void WriteBody(IHttpRequest request, Payload payload);
		String ContentType { get; }
	}
}
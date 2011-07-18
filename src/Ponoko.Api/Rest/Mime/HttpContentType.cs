using System;

namespace Ponoko.Api.Rest.Mime {
	public interface HttpContentType : IDisposable {
		void WriteBody(IHttpRequest request, Payload payload);
		String ContentType { get; }
	}
}
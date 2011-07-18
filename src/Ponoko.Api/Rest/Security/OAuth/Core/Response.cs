using System;
using System.IO;
using System.Net;

namespace Ponoko.Api.Rest.Security.OAuth.Core {
	public interface Response : IDisposable {
		HttpStatusCode StatusCode { get; }
		Stream Open();
	}

	public class SystemResponse : Response {
		private readonly HttpWebResponse _innerResponse;
		public SystemResponse(HttpWebResponse innerResponse) { _innerResponse = innerResponse; }

		// TODO: Consider elminating dependency on System.Net.
		public HttpStatusCode StatusCode {  get { return _innerResponse.StatusCode; } }
		public Stream Open() { return _innerResponse.GetResponseStream(); }
		public void Dispose() { _innerResponse.Close(); }
	}
}

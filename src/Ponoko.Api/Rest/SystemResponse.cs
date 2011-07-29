using System.IO;
using System.Net;

namespace Ponoko.Api.Rest {
	public sealed class SystemResponse : Response {
		private readonly HttpWebResponse _innerResponse;
		public SystemResponse(HttpWebResponse innerResponse) { _innerResponse = innerResponse; }

		// TODO: Consider elminating dependency on System.Net.
		public HttpStatusCode StatusCode {  get { return _innerResponse.StatusCode; } }
		public Stream Open() { return _innerResponse.GetResponseStream(); }
		public void Dispose() { _innerResponse.Close(); }
	}
}
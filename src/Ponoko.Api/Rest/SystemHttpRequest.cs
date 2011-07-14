using System;
using System.IO;
using System.Net;

namespace Ponoko.Api.Rest {
	public class SystemHttpRequest : IHttpRequest {
		private readonly HttpWebRequest _innerRequest = null;

		public string ContentType {
			get { return _innerRequest.ContentType; }
			set { _innerRequest.ContentType = value; }
		}

		public long ContentLength {
			set { _innerRequest.ContentLength = value; }
			get { return _innerRequest.ContentLength; }
		}

		public String Method {
			get { return _innerRequest.Method; }
			set { _innerRequest.Method = value; }
		}

		public Uri RequestUri {
			get { return _innerRequest.RequestUri; }
		}

		public Boolean AllowAutoRedirect {
			get { return _innerRequest.AllowAutoRedirect; }
			set { _innerRequest.AllowAutoRedirect = value; }
		}

		public Stream Open() {
			return _innerRequest.GetRequestStream();
		}

		public HttpWebResponse GetResponse() {
			return (HttpWebResponse)_innerRequest.GetResponse();
		}

		public void AddHeader(string name, string value) {
			_innerRequest.Headers.Add(name, value);
		}

		public SystemHttpRequest(Uri uri) {
			_innerRequest = (HttpWebRequest)WebRequest.Create(uri);
		}
	}
}
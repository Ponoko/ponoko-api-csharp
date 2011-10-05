using System;
using System.IO;
using System.Net;
using Ponoko.Api.Logging;
using Ponoko.Api.Rest.Mime;
using Ponoko.Api.Rest.Security;

namespace Ponoko.Api.Rest {
	public class SystemInternet : TheInternet {
		private readonly AuthorizationPolicy _authPolicy;
		private readonly Log _log;

		private Log Log { get { return _log; } }

		public SystemInternet(AuthorizationPolicy authPolicy) : this(authPolicy, new DevNullLog()) {}

		static SystemInternet() {
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
		}

		public SystemInternet(AuthorizationPolicy authPolicy, Log log) {
			_authPolicy = authPolicy;
			_log = log;
		}

		public override Response Get(Uri uri) {
			var unauthorized = new Request(RequestLine.Get(uri)) { ContentType = HttpContentTypeName.FormUrlEncoded };
			
			var request = AuthorizeAndConvert(unauthorized);

			return TryExecute(request);
		}

		public override Response Post(Uri uri, HttpContentType contentType, Payload payload) {
			var unauthorized = new Request(RequestLine.Post(uri), payload) { ContentType = contentType.ContentType };

			var authorized = AuthorizeAndConvert(unauthorized);
			
			AddBody(authorized, contentType, payload);

			return TryExecute(authorized);
		}

		private void AddBody(HttpWebRequest httpRequest, HttpContentType contentType, Payload payload) {
			using (var body = contentType.Format(payload)) {
				httpRequest.ContentLength = body.ContentLength;
				httpRequest.ContentType = body.ContentType;
				using (var outStream = httpRequest.GetRequestStream()) {
					Copy(body.Open(), outStream);
				}
			}
		}

		private void Copy(Stream from, Stream to) {
			const Int32 BUFFER_SIZE = 1024*512;

			var buffer = new Byte[BUFFER_SIZE];

			var reader = new BinaryReader(from);
			var writer = new BinaryWriter(to);
			var bytesRead = 0;
			while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0 ) {
				writer.Write(buffer, 0, bytesRead);
			}
		}

		private HttpWebRequest AuthorizeAndConvert(Request request) {
			var authorized = _authPolicy.Authorize(request);
			
			var result = Convert(authorized);
			result.Method = request.RequestLine.Verb;
			
			return result;
		}

		private HttpWebRequest Convert(Request authorized) {
			var result = (HttpWebRequest)WebRequest.Create(authorized.RequestLine.Uri);
			result.Headers.Add(authorized.Headers);

			return result;
		}

		private Response TryExecute(HttpWebRequest request) {
			Print(request);

			HttpWebResponse innerResponse;

			try {
				request.AllowAutoRedirect = false;
				innerResponse = (HttpWebResponse)request.GetResponse();
			} catch (WebException e) {
				if (e.Status != WebExceptionStatus.ProtocolError)
					throw;

				innerResponse = (HttpWebResponse)e.Response;
			}

			return new SystemResponse(innerResponse);
		}

		private void Print(HttpWebRequest request) {
			Log.Info("{0} {1}", request.Method, request.RequestUri.AbsoluteUri);
			Log.Info("Content-type: {0}", request.ContentType ?? "empty");
		}

		protected void Print(HttpWebResponse response) {
			Log.Info(Body(response));
		}

		private String Body(HttpWebResponse response) {
			using (var reader = new StreamReader(response.GetResponseStream())) {
				return reader.ReadToEnd();
			}
		}
	}
}

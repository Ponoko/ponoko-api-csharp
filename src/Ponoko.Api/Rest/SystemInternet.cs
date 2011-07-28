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

		public SystemInternet(AuthorizationPolicy authPolicy, Log log) {
			_authPolicy = authPolicy;
			_log = log;
		}

		public override Response Head(Uri uri) {
			var unauthorized = new Request(RequestLine.Head(uri)) { ContentType = new FormUrlEncoded().ContentType };

			var authorized = AuthorizeAndConvert(unauthorized);
			
			return TryExecute(authorized);
		}

		public override Response Get(Uri uri) {
			var unauthorized = Request.Get(uri);
			unauthorized.ContentType = new FormUrlEncoded().ContentType;
			
			var request = AuthorizeAndConvert(unauthorized);

			return TryExecute(request);
		}

		public override Response Post(Uri uri, HttpContentType contentType, Payload payload) {
			var unauthorized = new Request(RequestLine.Post(uri), payload) { ContentType = contentType.ContentType };

			var authorized = AuthorizeAndConvert(unauthorized);
			
			AddBody(authorized, contentType, payload);

			return TryExecute(authorized);
		}

		private void AddBody(IHttpRequest httpRequest, HttpContentType contentType, Payload payload) {
			using (var body = contentType.Format(payload)) {
				httpRequest.ContentLength = body.ContentLength;
				httpRequest.ContentType = body.ContentType;
				using (var outStream = httpRequest.Open()) {
					Copy(body.Open(), outStream);
				}
			}
		}

		private void Copy(Stream from, Stream to) {
			var buffer = new Byte[1024*512];

			var reader = new BinaryReader(from);
			var writer = new BinaryWriter(to);
			var bytesRead = 0;
			while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0 ) {
				writer.Write(buffer, 0, bytesRead);
			}
		}

		private SystemHttpRequest AuthorizeAndConvert(Request request) {
			var authorized = _authPolicy.Authorize(request);
			
			var result = Convert(authorized);
			result.Method = request.RequestLine.Verb;
			
			return result;
		}

		private SystemHttpRequest Convert(Request authorized) {
			var uriBuilder = new UriBuilder(authorized.RequestLine.Uri);

			var result = new SystemHttpRequest(uriBuilder.Uri) {Method = authorized.RequestLine.Verb};

			foreach (var key in authorized.Headers.Keys) {
				result.AddHeader(key.ToString(), authorized.Headers[key.ToString()]);
			}

			return result;
		}

		private Response TryExecute(SystemHttpRequest request) {
			Print(request);

			HttpWebResponse innerResponse;

			try {
				request.AllowAutoRedirect = false;
				innerResponse = request.GetResponse();
			} catch (WebException e) {
				if (e.Status != WebExceptionStatus.ProtocolError)
					throw;

				innerResponse = (HttpWebResponse)e.Response;
			}

			return new SystemResponse(innerResponse);
		}

		private void Print(SystemHttpRequest request) {
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

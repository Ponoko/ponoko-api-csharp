using System;
using System.IO;
using System.Net;
using Ponoko.Api.Logging;
using Ponoko.Api.Rest.Mime;

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

		public override Response Post(Uri uri, Payload payload) {
			var unauthorized = new Request(RequestLine.Post(uri), payload) {
				ContentType = SelectContentType(payload).ContentType
			};

			var authorized = AuthorizeAndConvert(unauthorized);
			
			AddBody(authorized, payload);

			return TryExecute(authorized);
		}

		private void AddBody(IHttpRequest httpRequest, Payload payload) {
			var contentType = SelectContentType(payload);

			using (contentType) {
				contentType.WriteBody(httpRequest, payload);
			}
		}

		private SystemHttpRequest AuthorizeAndConvert(Request request) {
			var authorized = _authPolicy.Authorize(request);
			
			var result = Convert(authorized);
			result.Method = request.RequestLine.Verb;
			
			return result;
		}

		private HttpContentType SelectContentType(Payload payload) {
			var thereAreFilesToSend = payload.DataItems.Count > 0;

			if (thereAreFilesToSend)
				return new MultipartFormData();

			return new FormUrlEncoded();
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

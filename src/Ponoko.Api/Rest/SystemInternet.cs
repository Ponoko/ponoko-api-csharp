﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Rest {
	public class SystemInternet : TheInternet {
		private readonly AuthorizationPolicy _authPolicy;

		public SystemInternet(AuthorizationPolicy authPolicy) {
			_authPolicy = authPolicy;
		}

		public override HttpWebResponse Head(Uri uri) {
			var request = AuthorizeAndConvert(new Request(RequestLine.Head(uri)));

			return TryExecute(request);
		}

		public override HttpWebResponse Get(Uri uri) {
			var unauthorized = Request.Get(uri);
			unauthorized.ContentType = new FormUrlEncoded().ContentType;
			var request = AuthorizeAndConvert(unauthorized);

			return TryExecute(request);
		}

		public override HttpWebResponse Post(Uri uri, Payload payload) {
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
			
			return Convert(authorized);
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

		private HttpWebResponse TryExecute(SystemHttpRequest request) {
			Print(request);
			
			try {
				request.AllowAutoRedirect = false;
				return request.GetResponse();
			} catch (WebException e) {
				if (e.Status != WebExceptionStatus.ProtocolError)
					throw;

				return (HttpWebResponse)e.Response;
			}   
		}

		private void Print(SystemHttpRequest request) {
			Console.WriteLine("{0} {1}", request.Method, request.RequestUri.AbsoluteUri);
			Console.WriteLine("Content-type: {0}", request.ContentType ?? "empty");
		}

		protected void Print(HttpWebResponse response) {
			Console.WriteLine(Body(response));
		}

		private String Body(HttpWebResponse response) {
			using (var reader = new StreamReader(response.GetResponseStream())) {
				return reader.ReadToEnd();
			}
		}
	}
}

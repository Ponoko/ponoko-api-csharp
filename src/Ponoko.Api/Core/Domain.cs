using System;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Core {
	public class Domain {
		protected TheInternet _internet;
		protected String _baseUrl;

		public Domain(TheInternet internet, String baseUrl) {
			_internet = internet;
			_baseUrl = baseUrl;
		}

		protected Uri Map(String format, params Object[] args) {
			var relativeUrl = String.Format(format, args);
			return new Uri(String.Format("{0}{1}", _baseUrl, relativeUrl));
		}

		protected Response Get(Uri uri) {
			var response = _internet.Get(uri);
			EnsureAuthorized(response);
			return response;
		}

		protected Response MultipartPost(Uri uri, Payload payload) {
			return Post(uri, new MultipartFormData(), payload);
		}

		protected Response Post(Uri uri, Payload payload) {
			return Post(uri, new FormUrlEncoded(), payload);
		}

		protected Response Post(Uri uri, HttpContentType contentType, Payload payload) {
			var response = _internet.Post(uri, contentType, payload);
			EnsureAuthorized(response);
			return response;
		}

		private void EnsureAuthorized(Response response) {
			var unauthorized = HttpStatusCode.Unauthorized;
			
			if (response.StatusCode == unauthorized) {
				var message = String.Format(
					"Authorization failed. " +
					"The server returned status {0} ({1}).", 
					unauthorized, 
					(Int32)unauthorized
				);

				throw new Exception(message);
			}
		}

		protected Exception Error(String message, Response response) {
			var json = ReadAll(response);
			var theError = TryDeserialize(json);

			return new Exception(String.Format(
				"{0}. The server returned status {1} ({2}), and error message: \"{3}\"", 
				message,
				response.StatusCode, 
				(Int32)response.StatusCode, 
				theError
			));
		}

		protected String ReadAll(Response response) {
			using (var rdr = new StreamReader(response.Open())) {
				return rdr.ReadToEnd();
			}
		}

		protected Error TryDeserialize(String json) {
			try {
				return ErrorDeserializer.Deserialize(json);
			} catch (Exception e) {
				throw new Exception(String.Format(
					"There was a problem deserializing the error message. The body of the response is: {0}", json), 
				     e
				);
			}
		}
	}
}
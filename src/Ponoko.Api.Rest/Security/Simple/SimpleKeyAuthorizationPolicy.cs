using System;
using System.Collections.Generic;
using System.Text;

namespace Ponoko.Api.Rest.Security.Simple {
	public class SimpleKeyAuthorizationPolicy : AuthorizationPolicy {
		private readonly SimpleKeyAuthorizationCredential _credential;

		public SimpleKeyAuthorizationPolicy(SimpleKeyAuthorizationCredential credential) {
			_credential = credential;
		}

		public Request Authorize(Request request) {
			var theQueryString = AddAuthParams(request.RequestLine.Parameters);

			var newRequestLine = NewRequestLine(request, theQueryString);

			return new Request(newRequestLine, request.Headers, request.Payload);
		}

		private RequestLine NewRequestLine(Request request, string theQueryString) {
			var theBaseUri = ToBaseUrl(request);

			var newUri = new Uri(theBaseUri + "?" + theQueryString);

			return new RequestLine(request.RequestLine.Verb, newUri, request.RequestLine.Version);
		}

		private UriBuilder ToBaseUrl(Request request) {
			return new UriBuilder(
				request.RequestLine.Uri.Scheme, 
				request.RequestLine.Uri.Host,
				request.RequestLine.Uri.Port,
				request.RequestLine.Uri.AbsolutePath
			);
		}

		private String AddAuthParams(List<Parameter> originalParams) {
			const string APP_KEY = "app_key";
			const string USER_ACCESS_KEY = "user_access_key";

			if (originalParams.Exists(it => it.Name == APP_KEY))
				throw new Exception("Request has already been authorized.");

			if (originalParams.Exists(it => it.Name == USER_ACCESS_KEY))
				throw new Exception("Request has already been authorized.");

			originalParams.Add(new Parameter { Name = APP_KEY, Value = _credential.AppKey });
			originalParams.Add(new Parameter { Name = USER_ACCESS_KEY, Value = _credential.UserAccessKey});

			var theQueryString = new StringBuilder();

			foreach (var parameter in originalParams) {
				theQueryString.AppendFormat("{0}={1}&", parameter.Name, parameter.Value);
			}

			return theQueryString.ToString().TrimEnd('&');
		}
	}
}
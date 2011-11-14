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

			var newUri = new Uri(theBaseUri + "?" + theQueryString.TrimEnd('&'));

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
			originalParams.Add(new Parameter { Name = "app_key", Value = _credential.AppKey });
			originalParams.Add(new Parameter { Name = "user_access_key", Value = _credential.UserAccessKey});

			var theQueryString = new StringBuilder();

			foreach (var parameter in originalParams) {
				theQueryString.AppendFormat("{0}={1}&", parameter.Name, parameter.Value);
			}

			return theQueryString.ToString();
		}
	}
}
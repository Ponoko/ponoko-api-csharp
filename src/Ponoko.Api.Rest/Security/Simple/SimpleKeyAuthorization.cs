using System;
using System.Text;

namespace Ponoko.Api.Rest.Security.Simple {
	public class SimpleKeyAuthorization : AuthorizationPolicy {
		private readonly SimpleKeyAuthorizationCredential _credential;

		public SimpleKeyAuthorization(SimpleKeyAuthorizationCredential credential) {
			_credential = credential;
		}

		public Request Authorize(Request request) {
			var originalParams = request.RequestLine.Parameters;
			originalParams.Add(new Parameter { Name = "app_key", Value = _credential.AppKey });
			originalParams.Add(new Parameter { Name = "user_access_key", Value = _credential.UserAccessKey});

			var theQueryString = new StringBuilder();

			foreach (var parameter in originalParams) {
				theQueryString.AppendFormat("{0}={1}&", parameter.Name, parameter.Value);
			}

			var theBaseUri = new UriBuilder(
				request.RequestLine.Uri.Scheme, 
				request.RequestLine.Uri.Host,
				request.RequestLine.Uri.Port,
				request.RequestLine.Uri.AbsolutePath
			);

			var newUri = new Uri(theBaseUri + "?" + theQueryString.ToString().TrimEnd('&'));

			var newRequestLine = new RequestLine(request.RequestLine.Verb, newUri, request.RequestLine.Version);
			
			return new Request(newRequestLine, request.Headers, request.Payload);
		}
	}
}
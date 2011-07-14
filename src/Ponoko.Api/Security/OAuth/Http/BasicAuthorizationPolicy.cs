using System.Collections.Specialized;
using Ponoko.Api.Rest;
using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Security.OAuth.Http {
	public class BasicAuthorizationPolicy : AuthorizationPolicy {
		public Request Authorize(Request request, CredentialSet credentials) {
			var parameters = new NameValueCollection {
				{"app_key", credentials.Consumer.Key},
			    {"user_access_key", credentials.Token.Key},
			    request.Payload.Parameters
			};

			return new Request(request.RequestLine, new NameValueCollection(0), new Payload(parameters));
		}
	}
}
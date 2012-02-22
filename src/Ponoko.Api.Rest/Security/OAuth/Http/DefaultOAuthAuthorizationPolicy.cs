using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Rest.Security.OAuth.Http {
	public class DefaultOAuthAuthorizationPolicy : AuthorizationPolicy {
		private readonly OAuthAuthorizationPolicy _innerOauthPolicy;
		
		public DefaultOAuthAuthorizationPolicy(CredentialSet credentials) {
			_innerOauthPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()), 
				credentials
			);
		}

		public Request Authorize(Request request) {
			return _innerOauthPolicy.Authorize(request);
		}
	}
}
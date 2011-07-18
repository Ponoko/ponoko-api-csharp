using System;
using System.Net;
using Ponoko.Api.Rest;
using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Security.OAuth.Http {
	public class OAuthAuthorizationPolicy : AuthorizationPolicy {
		private readonly OAuthHeader _oAuthHeader;
		private readonly CredentialSet _credentials;

		public OAuthAuthorizationPolicy(OAuthHeader oAuthHeader, CredentialSet credentials) {
			_oAuthHeader = oAuthHeader;
			_credentials = credentials;
		}

		public Request Authorize(Request request) {
			if (null == _credentials)
				throw new InvalidOperationException("Credentials are required.");

			request.Headers.Add("Authorization", GetAuthHeader(request, _credentials));
			return request;
		}

		private String GetAuthHeader(Request request, CredentialSet credentials) {
			return _oAuthHeader.New(request, credentials);
		}
	}
}
using System;
using System.Net;
using Ponoko.Net.Security.OAuth.Core;

namespace Ponoko.Net.Security.OAuth.Http {
	public class OAuthAuthorizationPolicy : AuthorizationPolicy {
		private readonly OAuthHeader _oAuthHeader;

		public OAuthAuthorizationPolicy(OAuthHeader oAuthHeader) {
			_oAuthHeader = oAuthHeader;
		}

		public Request Authorize(Request request, CredentialSet credentials) {
			if (null == credentials)
				throw new InvalidOperationException("Credentials are required.");

			var result = (HttpWebRequest) WebRequest.Create(request.RequestLine.Uri);
			result.Method = request.RequestLine.Verb;
			request.Headers.Add("Authorization", GetAuthHeader(request, credentials));
			return request;
		}

		private String GetAuthHeader(Request request, CredentialSet credentials) {
			return _oAuthHeader.New(request, credentials);
		}
	}
}
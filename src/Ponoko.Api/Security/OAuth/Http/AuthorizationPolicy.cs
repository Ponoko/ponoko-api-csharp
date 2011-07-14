using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Security.OAuth.Http {
	public interface AuthorizationPolicy {
		Request Authorize(Request request, CredentialSet credentials);
	}
}
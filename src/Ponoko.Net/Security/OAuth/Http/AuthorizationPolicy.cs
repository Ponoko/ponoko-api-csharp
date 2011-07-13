using Ponoko.Net.Security.OAuth.Core;

namespace Ponoko.Net.Security.OAuth.Http {
	public interface AuthorizationPolicy {
		Request Authorize(Request request, CredentialSet credentials);
	}
}
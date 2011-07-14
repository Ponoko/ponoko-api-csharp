using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Rest {
	public interface AuthorizationPolicy { Request Authorize(Request request); }
}
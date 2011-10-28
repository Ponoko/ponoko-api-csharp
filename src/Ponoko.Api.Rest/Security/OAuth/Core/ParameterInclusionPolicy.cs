using System;

namespace Ponoko.Api.Rest.Security.OAuth.Core {
	// http://oauth.net/core/1.0/#anchor14 9.1.1
	public static class ParameterInclusionPolicy {
		public static Boolean IncludeParameters(Request request) {
			return request.ContentType == "application/x-www-form-urlencoded";
		}
	}
}
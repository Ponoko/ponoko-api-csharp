using System;

namespace Ponoko.Api.Rest.Security.Simple {
	public class SimpleKeyAuthorizationCredential {
		public readonly string AppKey;
		public readonly string UserAccessKey;

		public SimpleKeyAuthorizationCredential(String appKey, String userAccessKey) {
			AppKey = appKey;
			UserAccessKey = userAccessKey;
		}
	}
}
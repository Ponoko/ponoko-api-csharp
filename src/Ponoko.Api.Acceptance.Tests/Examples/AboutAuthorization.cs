using System.Net;
using NUnit.Framework;
using Ponoko.Api.Security.OAuth.Core;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	public class AboutAuthorization : AcceptanceTest {
		[Test]
		public void invalid_credentials_returns_401() {
			var anyValidUri = Map("/nodes");

			var invalidCredentials = new CredentialSet(new Credential("xxx", "yyy"));

			using (var response = Get(anyValidUri, invalidCredentials)) {
				Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
			}
		}
	}
}

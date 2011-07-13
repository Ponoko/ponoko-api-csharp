using System.Net;
using NUnit.Framework;
using Ponoko.Net.Security.OAuth.Core;

namespace Ponoko.Net.Acceptance.Tests.Examples {
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

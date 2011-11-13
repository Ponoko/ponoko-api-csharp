using System;
using System.Configuration;
using Ponoko.Api.Acceptance.Tests.Examples;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.Simple;

namespace Ponoko.Api.Acceptance.Tests {
	public static class Settings {
		public static CredentialSet Credentials {
			get { return new CredentialSet(_consumer, _token); }
		}

		public static String BaseUrl { 
			get { return ConfigurationManager.AppSettings["Ponoko.Api.BaseUrl"]; }
		}

		public static SimpleKeyAuthorizationCredential SimpleKeyAuthorizationCredential {
			get { 
				return new SimpleKeyAuthorizationCredential(
					ConfigurationManager.AppSettings["Ponoko.SimpleKeyAuthorization.AppKey"], 
					ConfigurationManager.AppSettings["Ponoko.SimpleKeyAuthorization.UserAccessKey"]
				); 
			} 
		}

		private static readonly Credential _consumer = new Credential(
			ConfigurationManager.AppSettings["Ponoko.Consumer.Key"], 
			ConfigurationManager.AppSettings["Ponoko.Consumer.Secret"]
		);

		private static readonly Credential _token = new Credential(
			ConfigurationManager.AppSettings["Ponoko.Token.Key"], 
			ConfigurationManager.AppSettings["Ponoko.Token.Secret"]
		);
	}
}
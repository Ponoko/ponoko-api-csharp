using System;
using System.Configuration;
using Ponoko.Api.Rest.Security.OAuth.Core;

namespace Ponoko.Api.Acceptance.Tests {
	public static class Settings {
		public static CredentialSet Credentials {
			get { return new CredentialSet(_consumer, _token); }
		}

		public static String BaseUrl { 
			get { return ConfigurationManager.AppSettings["Ponoko.Api.BaseUrl"]; }
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
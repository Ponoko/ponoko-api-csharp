using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Security.OAuth.Core;
using Ponoko.Api.Security.OAuth.Http;
using Ponoko.Api.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests {
	public class AcceptanceTest {
		private readonly Credential _consumer = new Credential(
			ConfigurationManager.AppSettings["Ponoko.Consumer.Key"], 
			ConfigurationManager.AppSettings["Ponoko.Consumer.Secret"]
		);

		private readonly Credential _token = new Credential(
			ConfigurationManager.AppSettings["Ponoko.Token.Key"], 
			ConfigurationManager.AppSettings["Ponoko.Token.Secret"]
		);

		protected CredentialSet Credentials {
			get { return new CredentialSet(_consumer, _token); }
		}

		protected TheInternet Internet {
			get {
				return new TheInternet(
					new OAuthAuthorizationPolicy(
						new MadgexOAuthHeader(
							new SystemClock(), 
							new SystemNonceFactory()
						), 
						Credentials
					)
				);
			}
		}

		protected String BaseUrl { 
			get { return "https://sandbox.ponoko.com/services/api/v2"; }
		}

		protected Uri Map(String format, params Object[] args) {
			var relativeUrl = String.Format(format, args);
			return new Uri(String.Format("{0}{1}", BaseUrl, relativeUrl));
		}

		protected String Json(HttpWebResponse response) {
			return new Deserializer().Deserialize(Body(response)).ToString(Formatting.Indented);
		}

		protected String Body(HttpWebResponse response) {
			using (var reader = new StreamReader(response.GetResponseStream())) {
				return reader.ReadToEnd();
			}
		}

		protected HttpWebResponse Head(Uri uri) {
			return Head(uri, new NameValueCollection());
		}

		protected HttpWebResponse Head(Uri uri, NameValueCollection parameters) {
			return Internet.Head(uri, new Payload(parameters));
		}

		protected HttpWebResponse Get(Uri uri) {
			return Get(uri, Payload.Empty);
		}

		protected HttpWebResponse Get(Uri uri, Payload payload) {
			return Internet.Get(uri, payload);
		}

		protected HttpWebResponse Post(Uri uri, Payload payload) {
			return Internet.Post(uri, payload);
		}

		protected HttpWebResponse Options(Uri uri, Payload payload) {
			return Internet.Options(uri, payload);
		}

		[SetUp]
		protected void BeforeAll() {
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
		}
	}
}

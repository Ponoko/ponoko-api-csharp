using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests {
	public class AcceptanceTest {
		protected TheInternet Internet {
			get {
				return new SystemInternet(
					new OAuthAuthorizationPolicy(
						new MadgexOAuthHeader(
							new SystemClock(), 
							new SystemNonceFactory()
						), 
						Settings.Credentials
					)
				);
			}
		}

		protected Uri Map(String format, params Object[] args) {
			var relativeUrl = String.Format(format, args);
			return new Uri(String.Format("{0}{1}", Settings.BaseUrl, relativeUrl));
		}

		protected String Json(Response response) {
			return new Deserializer().Deserialize(Body(response)).ToString(Formatting.Indented);
		}

		protected String Body(Response response) {
			using (var reader = new StreamReader(response.Open())) {
				return reader.ReadToEnd();
			}
		}

		protected Response Head(Uri uri) {
			return Head(uri, new NameValueCollection());
		}

		protected Response Head(Uri uri, NameValueCollection parameters) {
			return Internet.Head(uri);
		}

		protected Response Get(Uri uri) { return Internet.Get(uri); }

		protected Response Post(Uri uri, Payload payload) {
			return Internet.Post(uri, payload);
		}

		[SetUp]
		protected void BeforeAll() {
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
		}
	}
}

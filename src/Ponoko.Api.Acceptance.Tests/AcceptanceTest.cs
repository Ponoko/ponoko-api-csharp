using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using Ponoko.Api.Json;
using Ponoko.Api.Logging;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Mime;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests {
	public class AcceptanceTest {
		protected TheInternet Internet {
			get {
				var authPolicy = new OAuthAuthorizationPolicy(
					new MadgexOAuthHeader(
						new SystemClock(),
						new SystemNonceFactory()
					),
					Settings.Credentials
				);
				
				var log = new ConsoleLog();

				return new SystemInternet(authPolicy, log);
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

		protected Response Get(Uri uri) { return Internet.Get(uri); }

		protected Response Post(Uri uri, HttpContentType contentType, Payload payload) {
			return Internet.Post(uri, contentType, payload);
		}
	}
}

using System;
using System.IO;
using Newtonsoft.Json;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Logging;
using Ponoko.Api.Rest.Mime;
using Ponoko.Api.Rest.Security.OAuth.Http;

namespace Ponoko.Api.Acceptance.Tests {
	public class AcceptanceTest {
		protected TheInternet Internet {
			get {
				var authPolicy = new DefaultOAuthAuthorizationPolicy(
					Settings.Credentials
				);

				return new SystemInternet(authPolicy, new ConsoleLog());
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

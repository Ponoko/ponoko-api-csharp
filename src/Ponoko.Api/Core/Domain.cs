using System;
using System.IO;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class Domain {
		protected TheInternet _internet;
		protected String _baseUrl;

		public Domain(TheInternet internet, String baseUrl) {
			_internet = internet;
			_baseUrl = baseUrl;
		}

		protected Uri Map(String format, params Object[] args) {
			var relativeUrl = String.Format(format, args);
			return new Uri(String.Format("{0}{1}", _baseUrl, relativeUrl));
		}

		protected String Get(Uri uri) {
			using (var response = _internet.Get(uri)) {
				return ReadAll(response);
			}
		}

		protected String ReadAll(Response response) {
			using (var rdr = new StreamReader(response.Open())) {
				return rdr.ReadToEnd();
			}
		}
	}
}
using System;
using Ponoko.Api.Rest.Mime;

namespace Ponoko.Api.Rest {
	public abstract class TheInternet {
		public abstract Response Get(Uri uri);
		public abstract Response Post(Uri uri, HttpContentType contentType, Payload payload);
	}
}
using System;
using System.Net;

namespace Ponoko.Api.Rest {
	public abstract class TheInternet {
		public HttpWebResponse Head(Uri uri) { return Head(uri, Payload.Empty);}
		public abstract HttpWebResponse Head(Uri uri, Payload payload);
		public HttpWebResponse Get(Uri uri) { return Get(uri, Payload.Empty); }
		public abstract HttpWebResponse Get(Uri uri, Payload payload);
		public abstract HttpWebResponse Post(Uri uri, Payload payload);
		public abstract HttpWebResponse Options(Uri uri, Payload payload);
	}
}
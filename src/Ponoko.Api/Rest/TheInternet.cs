using System;
using System.Net;

namespace Ponoko.Api.Rest {
	public abstract class TheInternet {
		public abstract HttpWebResponse Head(Uri uri);
		public abstract HttpWebResponse Get(Uri uri);
		public abstract HttpWebResponse Post(Uri uri, Payload payload);
	}
}
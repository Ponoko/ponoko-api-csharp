﻿using System;

namespace Ponoko.Api.Rest {
	public abstract class TheInternet {
		public abstract Response Head(Uri uri);
		public abstract Response Get(Uri uri);
		public abstract Response Post(Uri uri, Payload payload);
	}
}
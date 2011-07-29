using System;
using System.IO;
using System.Net;

namespace Ponoko.Api.Rest {
	public interface Response : IDisposable {
		HttpStatusCode StatusCode { get; }
		Stream Open();
	}
}

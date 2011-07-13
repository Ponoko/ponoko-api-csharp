using System;
using System.IO;
using System.Net;

namespace Ponoko.Net.Rest {
	public interface IHttpRequest {
		String ContentType { get; set; }
		Int64 ContentLength { get; set; }
		String Method { get; set; }
		Stream Open();
		void AddHeader(String name, String value);
		HttpWebResponse GetResponse();
	}
}
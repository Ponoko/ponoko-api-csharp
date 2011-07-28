using System;
using System.IO;

namespace Ponoko.Api.Rest.Mime {
	public interface HttpContentType : IDisposable {
		Body Format(Payload payload);
		String ContentType { get; }
	}

	public class Body : IDisposable {
		public Int64 ContentLength { get; set; }
		public String ContentType { get; set; }

		private Stream _in;

		public Stream In {
			get { return _in ?? (_in = new MemoryStream()); }
		}	

		public Stream Open() {
			In.Seek(0, SeekOrigin.Begin);
			return In;
		}

		public void Dispose() {
			if (_in != null) {
				_in.Close();
			}
		}
	}
}
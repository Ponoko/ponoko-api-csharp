using System;
using System.IO;

namespace Ponoko.Api.Rest.Mime {
	public class Body : IDisposable {
		private readonly Stream _backingStream;
		public Int64 ContentLength { get; set; }
		public String ContentType { get; set; }

		public Body(Stream backingStream) {
			_backingStream = backingStream;
		}

		public Stream In {
			get { return _backingStream; }
		}	

		public Stream Open() {
			In.Seek(0, SeekOrigin.Begin);
			return In;
		}

		public void Dispose() {
			if (_backingStream != null) {
				_backingStream.Close();
			}
		}
	}
}
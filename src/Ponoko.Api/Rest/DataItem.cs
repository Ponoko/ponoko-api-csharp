using System;
using System.IO;

namespace Ponoko.Api.Rest {
	public class DataItem {
		private Byte[] _data;
		private readonly FileInfo _file;

		public String ContentType { get; set; }
		public Byte[] GetBytes() { return Data ?? new Byte[0]; }
		public Int32 Length { get { return Data != null ? Data.Length : 0; } }

		public String FileName {
			get { return (null == _file) ? String.Empty : _file.Name; }
		}

		private Byte[] Data {
			get { return _data ?? (_data = File.ReadAllBytes(_file.FullName)); }
		}

		public DataItem(FileInfo file) : this(file, String.Empty) {}

		public DataItem(FileInfo file, String contentType) {
			_file = file;
			ContentType = contentType;
		}

		public DataItem(String name, Byte[] data, String contentType) : this(data) {
			ContentType = contentType;
		}

		public DataItem(Byte[] data) {
			ContentType = "unknown";
			_data = data;
		}
	}
}

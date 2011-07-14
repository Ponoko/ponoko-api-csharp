using System;
using System.IO;

namespace Ponoko.Api.Rest {
	public class DataItem {
		private Byte[] _data;
		private readonly FileInfo _file;

		public String ContentType { get; set; }
		public Byte[] GetBytes() { return Data ?? new Byte[0]; }
		public Int32 Length { get { return Data != null ? Data.Length : 0; } }
		public String Name { get; set; }

		public String FileName {
			get { return (null == _file) ? String.Empty : _file.Name; }
		}

		private Byte[] Data {
			get { return _data ?? (_data = File.ReadAllBytes(_file.FullName)); }
		}

		public DataItem(String name, FileInfo file, String contentType) {
			_file = file;
			Name = name;
			ContentType = contentType;
		}

		public DataItem(String name, Byte[] data, String contentType) : this(name, data) {
			ContentType = contentType;
		}

		public DataItem(String name, Byte[] data) {
			ContentType = "unknown";
			Name = name;
			_data = data;
		}

		public String AttributeString() {
			return String.Format("name=\"{0}\"; filename=\"{1}\"", Name, FileName);
		}
	}
}

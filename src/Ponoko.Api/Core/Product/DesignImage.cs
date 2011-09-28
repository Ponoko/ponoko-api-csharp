using System;
using System.IO;

namespace Ponoko.Api.Core.Product {
	public class DesignImage {
		private readonly string _contentType;
		private String _fullName;
		private String _fileName;

		public String Filename {
			get { return _fileName; }
			set { _fileName = _fullName = value; }
		}

		public DesignImage() : this(null, null) {}

		public DesignImage(FileInfo fileInfo) : this(fileInfo, null) {}

		public DesignImage(FileInfo fileInfo, String contentType) {
			_contentType = contentType;

			if (fileInfo != null) {
				_fullName = fileInfo.FullName;
				_fileName = fileInfo.Name;
			}
		}

		public String FullName { get { return _fullName ?? Filename; } }

		public string ContentType { get { return _contentType; } }
	}
}
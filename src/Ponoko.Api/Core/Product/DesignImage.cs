using System;
using System.IO;

namespace Ponoko.Api.Core.Product {
	public class DesignImage {
		private String _fullName;
		private String _fileName;

		public String Filename {
			get { return _fileName; }
			set { _fileName = _fullName = value; }
		}

		public DesignImage() : this(null) {}

		public DesignImage(FileInfo fileInfo) {
			if (fileInfo != null) {
				_fullName = fileInfo.FullName;
				_fileName = fileInfo.Name;
			}
		}

		public String FullName {
			get { return _fullName ?? Filename; }
		}
	}
}
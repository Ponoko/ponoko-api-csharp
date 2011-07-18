using System;
using System.Text;

namespace Ponoko.Api.Rest.Mime {
	public class MultipartFormDataDataItemFormatter {
		private readonly string _boundary;

		public MultipartFormDataDataItemFormatter(String boundary) {
			_boundary = boundary;
		}

		public String Header(DataItem dataItem) {
			var builder = new StringBuilder();
			builder.AppendFormat("--{0}\r\n", _boundary);
			
			var attributes = String.Format("name=\"{0}\"; filename=\"{1}\"", dataItem.Name, dataItem.FileName);

			builder.AppendFormat("Content-Disposition: form-data; {0}\r\n", attributes);
			builder.AppendFormat("Content-Type: {0}\r\n", dataItem.ContentType);
			builder.Append("Content-Transfer-Encoding: binary\r\n");
			builder.Append("\r\n");

			return builder.ToString();
		}

		public String NameValuePair(String name, String value) {
			var builder = new StringBuilder();
			builder.AppendFormat("--{0}\r\n", _boundary);
			builder.AppendFormat("Content-Disposition: form-data; name=\"{0}\"", name);
			builder.Append("\r\n\r\n");
			builder.AppendFormat("{0}\r\n", value);
			return builder.ToString();
		}
	}
}
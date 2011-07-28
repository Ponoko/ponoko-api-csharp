using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ponoko.Api.Rest.Mime {
	public class MultipartFormDataBodyBuilder {
		private readonly string _boundary;
		private readonly Encoding _encoding;
		private readonly Stream _stream;
		private readonly MultipartFormDataDataItemFormatter _formatter;

		public MultipartFormDataBodyBuilder(String boundary, Encoding encoding, Stream stream) {
			_boundary = boundary;
			_encoding = encoding;
			_stream = stream;
			_formatter = new MultipartFormDataDataItemFormatter(boundary);
		}

		public Stream Stream {
			get { return _stream; }
		}

		public void Append(Payload payload) {
			Append(payload.Parameters);
			Append(payload.DataItems);
		}

		private void Append(List<Parameter> parameters) {
			if (parameters.Count > 0) {
				var bodyBuilder = new StringBuilder();
				
				foreach (var parameter in parameters) {
					bodyBuilder.Append(Format(parameter.Name, parameter.Value));
				}

				Append(bodyBuilder.ToString());
			}
		}

		private void Append(ICollection<DataItem> dataItems) {
			if (dataItems.Count == 0) return; 
				
			foreach (var dataItem in dataItems) { Append(dataItem); }
		}

		private void Append(DataItem dataItem) {
			Append(Format(dataItem));

			if (dataItem.Length > 0) {
				Stream.Write(dataItem.GetBytes(), 0, dataItem.Length);
			}

			AppendNewline();
		}

		public	void AppendFooter()			{ Append("--" + _boundary + "--\r\n"); }
		public	void Flush()				{ Stream.Flush(); }
		private void AppendNewline()		{ Append("\r\n");}
		private void Append(String text)	{ Append(_encoding.GetBytes(text));}
		private void Append(Byte[] what)	{ Stream.Write(what, 0, what.Length); }

		private String Format(String name, String value) { return _formatter.NameValuePair(name, value); }
		private String Format(DataItem dataItem) { return _formatter.Header(dataItem); }
	}
}
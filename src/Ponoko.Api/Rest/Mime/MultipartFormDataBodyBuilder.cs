using System;
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

		// TODO, 2011-07-28: Now that we have one list of fields, we need to work out a way of preventing this if/else.
		// We could push that behaviour a field abstraction, so that each field knows how to append itself.
		// It depends whether we want protection against new objects or new functions.
		public void Append(Payload payload) {
			foreach (var field in payload) {
				if (field.Value != null && field.Value.GetType() == typeof(DataItem)) {
					Append((DataItem)field.Value);
				} else {
					Append(Format(field.Name, field.Value != null ? field.Value.ToString() : String.Empty));
				}
			}
		}

		private void Append(DataItem dataItem) {
			Append(FormatHeader(dataItem));

			if (dataItem.Length > 0) {
				Stream.Write(dataItem.GetBytes(), 0, dataItem.Length);
			}

			AppendNewline();
		}

		public	void AppendFooter()			{ Append("--" + _boundary + "--\r\n"); }
		private void AppendNewline()		{ Append("\r\n");}
		private void Append(String text)	{ Append(_encoding.GetBytes(text));}
		private void Append(Byte[] what)	{ Stream.Write(what, 0, what.Length); }
		
		public	void Flush()				{ Stream.Flush(); }
		private Stream Stream { get { return _stream; } }

		private String Format(String name, String value) { return _formatter.NameValuePair(name, value); }
		private String FormatHeader(DataItem dataItem) { return _formatter.Header(dataItem); }
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ponoko.Api.Rest.Mime {
	public class MultipartFormData : HttpContentType {
		private readonly MemoryStream _stream = new MemoryStream();
		private readonly Encoding _characterEncoding = Encoding.UTF8;
		private readonly MultipartFormDataDataItemFormatter _formatter;
		
		private String Boundary { get; set; }
		public String ContentType { get; private set; }
		private MemoryStream Stream { get { return _stream; } }
		public Encoding CharacterEncoding { get { return _characterEncoding; }}

		public MultipartFormData() {
			Boundary = "MULTIPART_BOUNDARY-" + DateTime.Now.ToString("yyyyMMddhHHmmss");
			ContentType = String.Format("multipart/form-data, boundary={0}", Boundary);
			_formatter = new MultipartFormDataDataItemFormatter(Boundary);
		}

		public void WriteBody(IHttpRequest request, Payload payload) {
			Append(payload);

			AppendFooter();

			Flush();

			request.ContentLength = Stream.Position+1;
			request.ContentType = ContentType;

			EmitTo(request);
		}

		private void Append(Payload payload) {
			Append(payload.Parameters);
			Append(payload.DataItems);
		}

		private void Append(List<Parameter> parameters) {
			var bodyBuilder = new StringBuilder();

			if (parameters.Count > 0) {
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

		private void AppendFooter()			{ Append("--" + Boundary + "--\r\n"); }
		private void AppendNewline()		{ Append("\r\n");}
		private void Append(String text)	{ Append(CharacterEncoding.GetBytes(text));}
		private void Append(Byte[] what)	{ Stream.Write(what, 0, what.Length); }

		private String Format(String name, String value) { return _formatter.NameValuePair(name, value); }
		private String Format(DataItem dataItem) { return _formatter.Header(dataItem); }

		private void EmitTo(IHttpRequest request) {
			using (var outWriter = new BinaryWriter(request.Open())) {
				outWriter.Write(Stream.GetBuffer(), 0, Convert.ToInt32(request.ContentLength));
			}
		}

		private void Flush() { Stream.Flush(); }

		public void Dispose() { Dispose(true); }

		private void Dispose(Boolean disposing) {
			if (disposing && Stream != null) {
				Stream.Dispose();
			}
		}
	}
}
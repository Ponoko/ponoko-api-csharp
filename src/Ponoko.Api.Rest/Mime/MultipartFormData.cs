using System;
using System.Text;
using Ponoko.Api.Core.IO;

namespace Ponoko.Api.Rest.Mime {
	public class MultipartFormData : HttpContentType {
		private String Boundary { get; set; }
		public String ContentType { get; private set; }

		public MultipartFormData() {
			Boundary = "MULTIPART_BOUNDARY-" + DateTime.Now.ToString("yyyyMMddhHHmmss");
			ContentType = String.Format("multipart/form-data, boundary={0}", Boundary);
		}

		public Body Format(Payload payload) {
			var body = NewBody();

			var builder = NewBodyBuilder(body);

			builder.Append(payload);
			builder.AppendFooter();
			builder.Flush();

			body.ContentLength = body.In.Position;
			body.ContentType = ContentType;

			return body;
		}

		private MultipartFormDataBodyBuilder NewBodyBuilder(Body body) {
			return new MultipartFormDataBodyBuilder(Boundary, Encoding.UTF8, body.Open());
		}

		private Body NewBody() {
			return new Body(BackingStream());
		}

		private TempFileStream BackingStream() {
			return new TempFileStream(new DefaultFileSystem());
		}
	}
}
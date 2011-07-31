using System;
using System.IO;
using System.Text;

namespace Ponoko.Api.Rest.Mime {
	public class MultipartFormData : HttpContentType {
		private String Boundary { get; set; }
		public String ContentType { get; private set; }

		public MultipartFormData() {
			Boundary = "MULTIPART_BOUNDARY-" + DateTime.Now.ToString("yyyyMMddhHHmmss");
			ContentType = String.Format("multipart/form-data, boundary={0}", Boundary);
		}

		public Body Format(Payload payload) {
			var body = new Body(new MemoryStream());

			var builder = new MultipartFormDataBodyBuilder(Boundary, Encoding.UTF8, body.Open());

			builder.Append(payload);
			builder.AppendFooter();
			builder.Flush();

			body.ContentLength = body.In.Position;
			body.ContentType = ContentType;

			return body;
		}
	}
}
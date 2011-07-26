using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using Ponoko.Api.Core;

namespace Ponoko.Api.Rest.Mime {
	// See: http://www.w3.org/TR/REC-html40/interact/forms.html#h-17.13.4.1
	public class FormUrlEncoded : HttpContentType {
		public string ContentType {
			get { return "application/x-www-form-urlencoded";  }
		}

		public void WriteBody(IHttpRequest request, Payload payload) {
			var serializedParams = ToQuery(payload.Parameters);

			request.ContentLength = Encoding.UTF8.GetByteCount(serializedParams);
			request.ContentType = ContentType;

			using (var writer = new StreamWriter(request.Open())) {
			    writer.Write(serializedParams);
			}
		}

		// TODO: Extension
		private String ToQuery(IEnumerable<Parameter> parameters) {
			var buffer = new StringBuilder();
			
			foreach (var parameter in parameters) {
				buffer.AppendFormat("{0}={1}&", UrlEncode(parameter.Name), UrlEncode(parameter.Value));
			}

			return buffer.ToString().TrimEnd('&');

			//return String.Join("&", 
			//    Array.ConvertAll(parameters, 
			//        key => String.Format("{0}={1}", UrlEncode(key), UrlEncode(parameters[key]))
			//    )
			//);
		}

		private String UrlEncode(Object what) {
			return Uri.EscapeDataString(what.ToString());
		}

		public void Dispose() { }
	}
}

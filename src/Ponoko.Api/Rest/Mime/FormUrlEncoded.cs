using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ponoko.Api.Rest.Mime {
	// See: http://www.w3.org/TR/REC-html40/interact/forms.html#h-17.13.4.1
	public class FormUrlEncoded : HttpContentType {
		public string ContentType {
			get { return "application/x-www-form-urlencoded";  }
		}

		public Body Format(Payload payload) {
			var serializedParams = ToQuery(payload.Parameters);

			var result = new Body {
				ContentLength = Encoding.UTF8.GetByteCount(serializedParams),
				ContentType = this.ContentType
			};

			var writer = new StreamWriter(result.Open());
			writer.Write(serializedParams);
			return result;
		}

		private String ToQuery(IEnumerable<Parameter> parameters) {
			var buffer = new StringBuilder();
			
			foreach (var parameter in parameters) {
				var theParameterHasAValue = parameter.Value != null;
				// TODO: I think the server end handles empty parameters differently. If you supply an empty parameter value you may get
				// "Invalid OAuth Request" error. Consider expressing this elsewhere.
				if (theParameterHasAValue) {
					buffer.AppendFormat("{0}={1}&", UrlEncode(parameter.Name), UrlEncode(parameter.Value));
				}
			}

			return buffer.ToString().TrimEnd('&');
		}

		private String UrlEncode(Object what) {
			if (null == what) return null;
			return Uri.EscapeDataString(what.ToString());
		}

		public void Dispose() { }
	}
}

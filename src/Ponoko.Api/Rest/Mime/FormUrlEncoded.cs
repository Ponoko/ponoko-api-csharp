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
			var serializedParams = ToQuery(payload);

			var theBytes = Encoding.UTF8.GetBytes(serializedParams);

			var result = new Body(new MemoryStream()) {
				ContentLength = theBytes.Length,
				ContentType = this.ContentType
			};

			var writer = new BinaryWriter(result.Open());
			writer.Write(theBytes);
			writer.Flush();

			return result;
		}

		private String ToQuery(IEnumerable<Field> fields) {
			var buffer = new StringBuilder();
			
			foreach (var field in fields) {
				var theParameterHasAName = field.Name != null;
				var theParameterHasAValue = field.Value != null;
				var shouldAddTheField = theParameterHasAName && theParameterHasAValue;

				// TODO: I think the server end handles empty parameters differently. If you supply an empty parameter value you may get
				// "Invalid OAuth Request" error. Consider expressing this elsewhere.
				if (shouldAddTheField) {
					buffer.AppendFormat("{0}={1}&", UrlEncode(field.Name), UrlEncode(field.Value));
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

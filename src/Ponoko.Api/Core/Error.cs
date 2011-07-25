using System;
using System.Collections.Generic;
using System.Text;

namespace Ponoko.Api.Core {
	public class Error {
		public override String ToString() {
			var buffer = new StringBuilder(Message);

			if (Errors != null) {
				foreach (var error in Errors) { buffer.AppendLine(error.Value); }
			}

			return buffer.ToString().Trim();
		}

		public String Message { get; set; }
		public List<ErrorMessage> Errors { get; set; }
	}
}
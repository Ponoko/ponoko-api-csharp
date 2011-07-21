using System;
using System.Collections.Generic;

namespace Ponoko.Api.Core {
	public class Error {
		public String Message { get; set; }
		public List<ErrorMessage> Errors { get; set; }
	}
}
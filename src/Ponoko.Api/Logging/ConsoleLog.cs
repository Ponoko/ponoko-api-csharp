using System;

namespace Ponoko.Api.Logging {
	public class ConsoleLog : Log {
		public void Info(string format, params object[] args) {
			Console.WriteLine(format, args);
		}
	}
}
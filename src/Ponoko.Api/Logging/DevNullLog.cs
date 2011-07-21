namespace Ponoko.Api.Logging {
	public class DevNullLog : Log {
		public void Info(string format, params object[] args) {}
	}
}
namespace Ponoko.Api.Rest.Logging {
	public class DevNullLog : Log {
		public void Info(string format, params object[] args) {}
	}
}
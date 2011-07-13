using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ponoko.Net.Rest {
	public class Payload {
		public NameValueCollection Parameters { get; private set; }
		public List<DataItem> DataItems { get; private set; }
		public static Payload Empty { get {return new Payload(); }}

		public Payload() : this(new NameValueCollection(0)) {}
		public Payload(NameValueCollection parameters) : this(parameters, new List<DataItem>(0)) {}
		public Payload(NameValueCollection parameters, List<DataItem> dataItems) {
			Parameters = parameters;
			DataItems = dataItems;
		}
	}
}
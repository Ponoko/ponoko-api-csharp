using System.Collections.Generic;
using Ponoko.Api.Core;

namespace Ponoko.Api.Rest {
	public class Payload {
		public List<Parameter> Parameters { get; private set; }
		public List<DataItem> DataItems { get; private set; }
		public static Payload Empty { get {return new Payload(); }}

		public Payload() : this(new List<Parameter>()) {}
		public Payload(List<Parameter> parameters) : this(parameters, new List<DataItem>(0)) {}
		public Payload(List<Parameter> parameters, List<DataItem> dataItems) {
			Parameters = parameters;
			DataItems = dataItems;
		}
	}
}
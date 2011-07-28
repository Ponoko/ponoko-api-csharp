using System.Collections.Generic;

namespace Ponoko.Api.Rest {
	public class Payload {
		private readonly List<Field> _fields = new List<Field>();
		public static Payload Empty { get {return new Payload(); }}
		public List<Field> Fields{ get { return _fields; } }

		public Payload() : this(new List<Field>()) {}
		public Payload(List<Field> fields) {
			_fields = fields;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ponoko.Api.Rest {
	public class Payload : IEnumerable<Field> {
		private readonly List<Field> _fields;
		public static Payload Empty { get {return new Payload(); }}

		public Payload() : this(new List<Field>()) {}
		public Payload(List<Field> fields) {
			_fields = fields;
		}

		public void Add(String name, Object value) { Add(new Field {Name = name, Value = value });}
		public void Add(Field field) { _fields.Add(field); }
		public void AddRange(IEnumerable<Field> fields) { _fields.AddRange(fields); }

		public Boolean Contains(Func<Field, Boolean> matching) {
			return Exists(matching);
		}

		public Boolean Exists(Func<Field, Boolean> matching) {
			foreach (var field in this) {
				if (matching(field))
					return true;
			}

			return false;
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public IEnumerator<Field> GetEnumerator() { return _fields.GetEnumerator(); }
	}
}
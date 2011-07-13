using System;
using System.Text;

namespace Ponoko.Net.Rest {
	public class Parameter {
		public string Name { get; private set; }
		public string Value { get; private set; }

		public Parameter() { }

		public Parameter(String name, String value) {
			Name = name;
			Value = value;
		}

		public Byte[] GetBytes() { return Encoding.UTF8.GetBytes(Value); }
	}
}

using System;
using System.Collections.Generic;

namespace Ponoko.Api.Json.Generic {
	internal class ListDeserializer<T> {
		private readonly Func<string, T> _deserializer;
		private readonly string _collectionKeyName;

		public ListDeserializer(Func<String, T> deserializer, String collectionKeyName) {
			_deserializer = deserializer;
			_collectionKeyName = collectionKeyName;
		}

		public T[] Deserialize(String json) {
			var theList = new Deserializer().Deserialize(json);
			var result = new List<T>();

			foreach (var orderJson in theList[_collectionKeyName].Children()) {
				result.Add(DeserializeSingle(orderJson.ToString()));
			}

			return result.ToArray();
		}

		private T DeserializeSingle(String json) {
			return _deserializer(json);
		}
	}
}
using System;
using System.Collections.Generic;
using Ponoko.Api.Core;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Json {
	public static class NodeListDeserializer {
		public static Node[] Deserialize(String json) {
			var theList = new Deserializer().Deserialize(json);
			var result = new List<Node>();

			foreach (var nodeJson in theList["nodes"].Children()) {
				result.Add(SimpleDeserializer<Node>.Deserialize(nodeJson.ToString()));
			}

			return result.ToArray();
		}
	}
}
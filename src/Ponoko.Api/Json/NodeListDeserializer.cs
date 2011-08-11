using System;
using Ponoko.Api.Core;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Json {
	public static class NodeListDeserializer {
		public static Node[] Deserialize(String json) {
			return new ListDeserializer<Node>(SimpleDeserializer<Node>.Deserialize, "nodes").
				Deserialize(json);
		}
	}
}
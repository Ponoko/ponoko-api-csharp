using System;
using System.Collections.Generic;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class Nodes : Domain {
		public Nodes(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }
		
		public IList<Node> FindAll() {
			var json = Get(Map("/nodes"));

			return NodeListDeserializer.Deserialize(json);	
		}
	}
}
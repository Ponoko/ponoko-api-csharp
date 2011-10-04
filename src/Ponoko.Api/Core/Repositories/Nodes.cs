using System;
using System.Collections.Generic;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Repositories {
	public class Nodes : Domain {
		public Nodes(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }
		
		public IList<Node> FindAll() {
			using (var response = _internet.Get(Map("/nodes"))) {
				if (response.StatusCode == HttpStatusCode.OK) 
					return NodeListDeserializer.Deserialize(ReadAll(response));

				throw Error("Failed to find nodes.", response);
			}
		}
	}
}
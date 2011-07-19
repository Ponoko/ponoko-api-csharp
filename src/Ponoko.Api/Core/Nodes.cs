using System;
using System.Collections.Generic;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class Nodes : Domain {
		public Nodes(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }
		
		public IList<Node> FindAll() {
			using (var response = _internet.Get(Map("/nodes"))) {
				if (response.StatusCode == HttpStatusCode.OK) 
					return NodeListDeserializer.Deserialize(ReadAll(response));

				throw Error(response);
			}
		}

		private Exception Error(Response response) {
			var theError = new Deserializer().Deserialize(ReadAll(response))["error"].Value<String>("message");

			return new Exception(String.Format(
				"{0}. The server returned status {1} ({2}).", 
				theError, 
				response.StatusCode, 
				(Int32)response.StatusCode)
			);
		}
	}
}
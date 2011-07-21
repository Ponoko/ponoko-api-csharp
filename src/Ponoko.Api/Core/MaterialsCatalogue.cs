using System;
using System.Collections.Generic;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class MaterialsCatalogue : Domain {
		public MaterialsCatalogue(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public IList<Material> FindAll(String nodeKey) {
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			var json = Get(uri);

			return MaterialListDeserializer.Deserialize(json);
		}
	}
}
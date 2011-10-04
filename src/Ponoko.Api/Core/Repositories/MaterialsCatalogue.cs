using System;
using System.Collections.Generic;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Repositories {
	public class MaterialsCatalogue : Domain {
		public MaterialsCatalogue(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public IList<Material> FindAll(String nodeKey) {
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			using (var response = Get(uri)) {
				var json = ReadAll(response);
				return MaterialCatalogueDeserializer.Deserialize(json);
			}
		}
	}
}
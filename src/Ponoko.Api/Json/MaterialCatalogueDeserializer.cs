using System;
using Ponoko.Api.Core;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Json {
	public static class MaterialCatalogueDeserializer {
		public static Material[] Deserialize(String json) {
			return new ListDeserializer<Material>(MaterialDeserializer.Deserialize, "materials").
				Deserialize(json);
		}
	}
}
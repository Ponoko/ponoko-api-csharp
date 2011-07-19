﻿using System;
using System.Collections.Generic;
using Ponoko.Api.Core;

namespace Ponoko.Api.Json {
	public static class MaterialListDeserializer {
		public static Material[] Deserialize(String json) {
			var theList = new Deserializer().Deserialize(json);
			var result = new List<Material>();

			foreach (var materialJson in theList["materials"].Children()) {
				result.Add(MaterialDeserializer.Deserialize(materialJson.ToString()));
			}

			return result.ToArray();
		}
	}
}
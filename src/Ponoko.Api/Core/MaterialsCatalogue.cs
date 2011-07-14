﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core {
	public class MaterialsCatalogue {
		private readonly TheInternet _internet;
		private readonly String _baseUrl;

		public MaterialsCatalogue(TheInternet internet, String baseUrl) {
			_internet = internet;
			_baseUrl = baseUrl;
		}

		public IList<Material> FindAll(String nodeKey) {
			var uri = Map("{0}/{1}", "/nodes/material-catalog", nodeKey);

			var json = Get(uri);

			return MaterialListDeserializer.Deserialize(json);
		}

		private String Get(Uri uri) {
			var response = _internet.Get(uri);
			return ReadAll(response);
		}

		private string ReadAll(HttpWebResponse response) {
			using (var rdr = new StreamReader(response.GetResponseStream())) {
				return rdr.ReadToEnd();
			}
		}

		private Uri Map(String format, params Object[] args) {
			var relativeUrl = String.Format(format, args);
			return new Uri(String.Format("{0}{1}", _baseUrl, relativeUrl));
		}
	}
}
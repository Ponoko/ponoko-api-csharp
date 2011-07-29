﻿using System;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.Product.Commands {
	public class DeleteResourceCommand : Domain {
		public DeleteResourceCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }
		
		public void Delete(Uri uri) {
			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw new Exception("Delete failed");
				});

				Verify(response);
			}
		}

		private void Verify(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response));

			var deleted = json.Value<String>("deleted");
			var wasDeletedOkay = (deleted == "true");

			un.less(wasDeletedOkay, () => {
				throw new Exception(String.Format(
					"Delete failed. Expected the deleted flag to be true. but it was \"{0}\".", 
			        deleted
				));
			});
		}
	}
}
using System;
using System.Net;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.Product.Commands {
	public class DeleteDesignCommand : Domain {
		public DeleteDesignCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) { }

		public Product Delete(String productKey, String designKey) {
			var uri = Map("/products/{0}/delete-design/{1}", productKey, designKey);

			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw Error("Delete failed", response);
				});

				var json = new Deserializer().Deserialize(ReadAll(response))["product"].ToString();
				return ProductDeserializer.Deserialize(json);
			}
		}
	}
}

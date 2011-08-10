using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.Product.Commands {
	public class DeleteResourceCommand : Domain {
		public DeleteResourceCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) { }
		
		public void Delete(Uri uri) {
			using (var response = Post(uri, Payload.Empty)) {
				un.less(() => response.StatusCode == HttpStatusCode.OK, () => {
					throw Error("Delete failed", response);
				});

				Verify(response);
			}
		}

		private void Verify(Response response) {
			var json = new Deserializer().Deserialize(ReadAll(response));

			Ensure(json);

			var deleted = json.Value<String>("deleted");
			var wasDeletedOkay = (deleted == "true");

			un.less(wasDeletedOkay, () => {
				throw new Exception(String.Format(
					"Delete failed. Expected the deleted flag to be true. but it was \"{0}\".", 
			        deleted
				));
			});
		}

		private void Ensure(JObject json) {
			var hasTheFlag = HasDeletedFlag(json);

			un.less (hasTheFlag, () => {
				throw new Exception(String.Format(
         			"Delete failed. " +
         			"Expected the response to contain a deleted flag, but it does not. " +
         			"The server returned: {0}",
         			json
				));
			});
		}

		private bool HasDeletedFlag(JObject json) {
			var hasTheFlag = false;

			foreach (JProperty property in json.Properties()) {
				if (property.Name == "deleted") {
					hasTheFlag = true;
					break;
				}
			}

			return hasTheFlag;
		}
	}
}
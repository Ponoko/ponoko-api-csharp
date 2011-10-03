using System;
using System.Collections.Generic;
using System.IO;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class ProductFileRepository : ProductRepository {
		private readonly string _resource;

		public ProductFileRepository(TheInternet internet, String baseUrl, String resource)
			: base(internet, baseUrl) {
			_resource = resource;
		}

		public Product Add(String productKey, params File[] files) {
			var uri = Map("/products/{0}/{1}", productKey, _resource);

			var payload = ToPayload(files);

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Payload ToPayload(IEnumerable<File> files) {
			var payload = new Payload();

			foreach (var file in files) {
				payload.Add(
					PayloadName,
					new DataItem(new FileInfo(file.FullName), file.ContentType)
				);
			}

			return payload;
		}

		private String PayloadName {
			get { return String.Format("{0}[][uploaded_data]", _resource.Replace("-", "_")); }
		}

		public Stream Get(String productKey, String filename) {
			var uri = Map("/products/{0}/{1}/download?filename={2}", productKey, _resource, filename);

			return Get(uri).Open();
		}

		public Product Remove(String productKey, String filename) {
			var uri = Map("/products/{0}/{1}/destroy?filename={2}", productKey, _resource, filename);

			using (var response = Get(uri)) {
				return Deserialize(response);
			}
		}
	}
}
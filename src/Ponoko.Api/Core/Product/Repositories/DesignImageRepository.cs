using System;
using System.IO;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class DesignImageRepository : Domain {
		private readonly ProductFileRepository _fileRepository;

		public DesignImageRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {
			_fileRepository = new ProductFileRepository(internet, baseUrl, "design-images");
		}
		
		public Product Add(String productKey, params File[] files) {
			return _fileRepository.Add(productKey, files);
		}

		public Stream Get(String productKey, String filename) {
			return _fileRepository.Get(productKey, filename);
		}

		public Product Remove(String productKey, string filename) {
			return _fileRepository.Remove(productKey, filename);
		}
	}
}
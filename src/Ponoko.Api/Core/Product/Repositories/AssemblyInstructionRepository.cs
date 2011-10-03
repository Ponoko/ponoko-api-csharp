using System;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Repositories {
	public class AssemblyInstructionRepository : Domain {
		private readonly ProductFileRepository _fileRepository;

		public AssemblyInstructionRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {
			_fileRepository = new ProductFileRepository(internet, baseUrl, "assembly-instructions");
		}
		
		public Product Add(String productKey, params File[] files) {
			return _fileRepository.Add(productKey, files);
		}
	}
}
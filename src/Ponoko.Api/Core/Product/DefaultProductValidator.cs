using System;
using System.IO;
using Ponoko.Api.Core.IO;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Core.Product {
	public class DefaultProductValidator : ProductValidator {
		private readonly ReadonlyFileSystem _fileSystem;

		public DefaultProductValidator() {
			_fileSystem = new DefaultReadonlyFileSystem();
		}

		public void Validate(ProductSeed seed) {
			if (String.IsNullOrEmpty(seed.Name) || seed.Name.Trim().Equals(String.Empty))
				throw new ArgumentException("Cannot create a product without a name.", "seed");
		}

		public void Validate(params Design[] designs) {
			foreach (var design in designs) {
				Validate(design);
			}
		}

		private void Validate(Design design) {
			if (null == design)
				throw new ArgumentException("Cannot create a product without at least one Design.", "design");
			
			if (null == design.Filename)
				throw new ArgumentException("Cannot create a product unless the Design has a file.", "design");

			var theDesignFileExistsOnDisk = FileExists(design);

			un.less(theDesignFileExistsOnDisk, () => {
           		throw new FileNotFoundException(
           			"Cannot create a product unless the Design has a file that exists on disk. " + 
           			"Unable to find file \"" + design.Filename + "\""
				);
			});
		}

		private Boolean FileExists(Design design) { return _fileSystem.Exists(design.Filename); }
	}
}
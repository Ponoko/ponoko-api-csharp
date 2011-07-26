namespace Ponoko.Api.Core.Product.Commands {
	public interface ProductValidator {
		void Validate(ProductSeed seed);
		void Validate(params Design[] designs);
	}
}
namespace Ponoko.Api.Core.Product {
	public interface ProductValidator {
		void Validate(ProductSeed seed);
		void Validate(params Design[] designs);
	}
}
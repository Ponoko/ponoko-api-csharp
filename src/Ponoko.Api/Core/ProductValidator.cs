namespace Ponoko.Api.Core {
	public interface ProductValidator {
		void Validate(ProductSeed seed);
		void Validate(params Design[] designs);
	}
}
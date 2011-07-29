using System;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class DeleteDesignCommand : Domain {
		public DeleteDesignCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}

		public void Delete(String productKey, String designKey) {
			var uri = Map("/products/{0}/delete-design/{1}", productKey, designKey);
			
			new DeleteResourceCommand(_internet, _baseUrl).Delete(uri);
		}
	}
}

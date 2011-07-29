using System;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Product.Commands {
	public class DeleteCommand : Domain {
		public DeleteCommand(TheInternet internet, string baseUrl) : base(internet, baseUrl) {}
		
		public void Delete(String id) {
			var uri = Map("/products/delete/{0}", id);

			new DeleteResourceCommand(_internet, _baseUrl).Delete(uri);
		}
	}
}

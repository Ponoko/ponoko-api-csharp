using System;
using Ponoko.Api.Core.Shipping;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Core.Orders {
	public class FindCommand : Domain {
		public FindCommand(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public Order[] All() {
			var uri = Map("/orders");

			using (var response = Get(uri)) {
				Console.WriteLine(ReadAll(response));
			}

			return new Order[0];
		}
	}
}

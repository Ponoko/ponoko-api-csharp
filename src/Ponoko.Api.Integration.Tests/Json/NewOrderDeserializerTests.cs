using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class NewOrderDeserializerTests {
		[Test]
		public void tell_me_how_do_I_feel() {
			var json = "{" + 
				"'order': {" +
				"	'created_at': '2011/08/10 09:05:58 +0000', " +
				"	'cost': {" +
				"		'making': '16.02'," +
				"		'shipping': '88.67'," +
				"		'materials': '2.83'" +
				"	}," +
				"	'updated_at': '2011/08/10 09:06:04 +0000'," +
				"	'events': [" +
				"		{" +
				"			'completed_at': '2011/08/10 09:05:58 +0000'," +
				"			'name': 'order_received'" +
				"		}" +
				"	]," +
				"	'tracking_numbers': []," +
				"	'products': [" +
				"		{" +
				"			'quantity': 1," +
				"			'ref': null," +
				"			'key': 'c234ef00489012d76610949d0680b3e4'" +
				"		}" +
				"	]," +
				"	'node_key': '2e9d8c90326e012e359f404062cdb04a'," +
				"	'shipping_option_code': 'ups_worldwide_expedited'," +
				"	'ref': 'any reference'," +
				"	'key': 'c60b169a12c44412de9bdc832985a402'," +
				"	'shipped': false" +
			  "}" +
			"}";

			var result = OrderDeserializer.Deserialize(json);

			Assert.AreEqual(new DateTime(2011, 8, 10, 21, 5, 58), result.CreatedAt);
			Assert.That(result.TrackingNumbers, Is.Empty, "Expected zero tracking numbers");
			Assert.AreEqual("c234ef00489012d76610949d0680b3e4", result.Products.First().Key);
			Assert.AreEqual("order_received", result.Events.First().Name);
			
			Assert.AreEqual("2e9d8c90326e012e359f404062cdb04a", result.NodeKey, "Unexpected node key");
			Assert.AreEqual("ups_worldwide_expedited", result.ShippingOptionCode, "Unexpected shipping option");
			Assert.AreEqual("any reference", result.Reference, "Unexpected reference");
			Assert.AreEqual("c60b169a12c44412de9bdc832985a402", result.Key, "Unexpected key");
			Assert.IsFalse(result.HasShipped, "Expected that the order has not yet shipped");
		}
	}
}

using System;
using System.Linq;
using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class NewOrderDeserializerTests {
		[Test, Description("Can deserialize a single order")]
		public void tell_me_how_do_I_feel() {
			var json = "{" + 
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

		[Test]
		public void can_deserialize_a_list_of_orders() {
			var json = "{" +
	           "	'orders': [" +
	           "		{" +
	           "			'created_at': '2011/08/10 21:41:20 +0000'," +
	           "			'updated_at': '2011/08/10 21:41:24 +0000'," +
	           "			'node_key': '2e9d8c90326e012e359f404062cdb04a'," +
	           "			'ref': 'e080d218-f358-46e9-8309-222874e7805c'," +
	           "			'key': '0bd8822ef8deca6f41e70e88f9dbc1ec'," +
	           "			'shipped': false" +
	           "		}," +
	           "		{" +
	           "			'created_at': '2011/08/10 20:42:57 +0000'," +
	           "			'updated_at': '2011/08/10 20:43:02 +0000'," +
	           "			'node_key': '2e9d8c90326e012e359f404062cdb04a'," +
	           "			'ref': 'cd63b975-60b9-4036-830c-ba228a35ad4b'," +
	           "			'key': '36a91cc83b3a8531a25beabe490e375e'," +
	           "			'shipped': false" +
	           "		}" +
	           "	]" +
	           "}";

			var result = OrderListDeserializer.Deserialize(json);

			Assert.AreEqual(2, result.Length, "Expected two orders to be deserialized");
			Assert.AreEqual("0bd8822ef8deca6f41e70e88f9dbc1ec", result.First().Key, "Expected two orders to be deserialized");
		}
	}
}

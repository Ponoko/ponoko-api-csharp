using NUnit.Framework;
using Ponoko.Api.Json;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class ShippingOptionsDeserializerTests {
		[Test]
		public void can_deserialize() {
			var json = "{" + 
				"	'shipping_options':" + 
				"		{" + 
				"			'products':[" + 
				"				{ " +
				"					'quantity':1," + 
				"					'product_key':'cdc7b518457eecdfb2d96ac43702fcc6'" + 
				"				}" + 
				"			]," +
				"			'currency':'USD'," + 
				"			'options':[" + 
				"				{" + 
				"					'price':'88.67'," + 
				"					'name':'UPS Worldwide Expedited'," + 
				"					'code':'ups_worldwide_expedited'" + 
				"				}," + 
				"				{" + 
				"					'price':'93.42'," + 
				"					'name':'UPS Saver'," + 
				"					'code':'ups_saver'" + 
				"				}" + 
				"			]" + 
				"		}" + 
				"}";

			var result = ShippingOptionsDeserializer.Deserialize(json);

			Assert.AreEqual("USD", result.Currency, "Unexpected currency.");
			
			Assert.AreEqual(1, result.Products.Count, "Expected just one product info returned.");
			Assert.AreEqual("cdc7b518457eecdfb2d96ac43702fcc6", result.Products[0].Key, "The product has an unexpected key.");
			Assert.AreEqual(1, result.Products[0].Quantity, "The product has an unexpected key.");
			
			Assert.AreEqual(2, result.Options.Count, "Unexpected two options.");
			Assert.AreEqual(88.67d, result.Options[0].Price, "The first option has incorrect price.");
			Assert.AreEqual(93.42d, result.Options[1].Price, "The second option has incorrect price.");
		}
	}
}

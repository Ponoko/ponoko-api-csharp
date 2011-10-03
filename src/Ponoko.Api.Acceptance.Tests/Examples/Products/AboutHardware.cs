using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Product.Commands;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	[TestFixture]
	public class AboutHardware : ProductAcceptanceTest {
		private Product AnyProduct;
		private HardwareRepository _hardwareRepository;
		
		private HardwareRepository DesignImageRepository {
			get { return _hardwareRepository ?? (_hardwareRepository = new HardwareRepository(Internet, Settings.BaseUrl)); }
		}

		[SetUp]
		new public void BeforeEach() {
			AnyProduct = NewProduct("Example for testing hardware");
		}

		[TearDown]
		public void AfterEach() {
			if (AnyProduct != null) {
				new DeleteCommand(Internet, Settings.BaseUrl).Delete(AnyProduct.Key);
			}
		}

		[Test]
		public void you_can_add_hardware_to_a_product() {
			const String VALID_SKU = "GPS-08254";

			var result = DesignImageRepository.Add(AnyProduct.Key, VALID_SKU, 1);
			
			Assert.AreEqual(1, result.Hardware.Count, "Expected one hardware item.");
			Assert.AreEqual("GPS-08254", result.Hardware.First().Sku, "Unexpected hardware");
		}

		[Test, Ignore("PENDING")]
		public void you_can_find_hardware_in_the_materials_catalogue() {
			
		}
	}

	public class HardwareRepository : Domain {
		public HardwareRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}

		public Product Add(String productKey, String sku, Int32 quantity) {
			var uri = Map("/products/{0}/hardware", productKey);

			var payload = new Payload {
				{"sku", sku},
			    {"quantity", quantity}
			};

			using (var response = Post(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Product Deserialize(Response response) {
			if (response.StatusCode != HttpStatusCode.OK)
				throw Error("Unexpected status returned.", response);

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}

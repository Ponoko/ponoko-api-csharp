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

		// See: http://www.ponoko.com/make-and-sell/show-hardware/489-a-antenna-gps-3v-magnetic-mount-mcx?source_node=ponoko_united_states
		private const String VALID_SKU = "GPS-08254"; 
		
		private HardwareRepository HardwareRepository {
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
			var result = HardwareRepository.Add(AnyProduct.Key, VALID_SKU, 1);
			
			Assert.AreEqual(1, result.Hardware.Count, "Expected one hardware item.");
			Assert.AreEqual("GPS-08254", result.Hardware.First().Sku, "Unexpected hardware");
		}

		[Test]
		public void you_can_update_hardware_quantity() {
			var result = HardwareRepository.Add(AnyProduct.Key, VALID_SKU, 1);
			
			Assert.AreEqual(1, result.Hardware.Count, "Expected one hardware item");

			Assert.AreEqual(1, result.Hardware.First().Quantity, "Unexpected quantity for the first hardware item");
	
			result = HardwareRepository.Add(AnyProduct.Key, VALID_SKU, 1337);
			
			Assert.AreEqual(1, result.Hardware.Count, 
				"Unexpected number of hardware items returned. " + 
				"Expected the count to remain the same because we are updating."
			);
			Assert.AreEqual(1337, result.Hardware.First().Quantity, "Expected the count to have been updated");
		}

		[Test]
		public void you_can_remove_hardware() {
			var result = HardwareRepository.Add(AnyProduct.Key, VALID_SKU, 1);
			Assert.AreEqual(1, result.Hardware.Count, "Expected one hardware item");

			HardwareRepository.Remove(AnyProduct.Key, VALID_SKU);
			Assert.AreEqual(0, result.Hardware.Count, "Expected the hardware item to have been removed, but it's still there");
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

		public Product Remove(String productKey, String sku) {
			var uri = Map("/products/{0}/hardware", productKey);

			var payload = new Payload { {"sku", sku} };

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

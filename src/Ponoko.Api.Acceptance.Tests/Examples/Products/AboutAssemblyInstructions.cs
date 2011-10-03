using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using File = Ponoko.Api.Core.Product.File;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class AboutAssemblyInstructions : ProductAcceptanceTest {
		private Product AnyProduct;
		private ProductFileRepository _assemblyInstructionRepository;

		public ProductFileRepository AssemblyInstructionRepository {
			get {
				return _assemblyInstructionRepository ??(
					_assemblyInstructionRepository = new ProductFileRepository(Internet, Settings.BaseUrl, "assembly-instructions")
				);
			}
		}

		[SetUp]
		new public void BeforeEach() {
			AnyProduct = NewProduct("Example for testing assembly instructions");
		}

		[Test]
		public void you_can_add_assembly_instructions_to_a_product() {
			var theImage = new File(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

			Assert.AreEqual(1, theProduct.AssemblyInstructions.Count, "Expected one assembly instructions file");
		}
	}

	public class ProductFileRepository : Domain {
		private readonly string _resource;

		public ProductFileRepository(TheInternet internet, String baseUrl, String resource) : base(internet, baseUrl) {
			_resource = resource;
		}

		public Product Add(String productKey, params File[] files) {
			var uri = Map("/products/{0}/{1}", productKey, _resource);

			var payload = ToPayload(files);

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Payload ToPayload(IEnumerable<File> designImages) {
			var payload = new Payload();

			foreach (var designImage in designImages) {
				payload.Add(
					"assembly_instructions[][uploaded_data]", 
					new DataItem(new FileInfo(designImage.FullName), designImage.ContentType)
				);
			}

			return payload;
		}

		private Product Deserialize(Response response) {
			if (response.StatusCode != HttpStatusCode.OK)
				throw Error("Invalid status returned", response);

			var json = ReadAll(response);

			var productJson = new Deserializer().Deserialize(json)["product"];
			return ProductDeserializer.Deserialize(productJson.ToString());
		}
	}
}

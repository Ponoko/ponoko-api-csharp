using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class AboutAssemblyInstructions : ProductAcceptanceTest {
		private Product AnyProduct;
		private AssemblyInstructionRepository _assemblyInstructionRepository;

		public AssemblyInstructionRepository AssemblyInstructionRepository {
			get {
				return _assemblyInstructionRepository ??(
					_assemblyInstructionRepository = new AssemblyInstructionRepository(Internet, Settings.BaseUrl)
				);
			}
		}

		[SetUp]
		new public void BeforeEach() {
			AnyProduct = NewProduct("Example for testing assembly instructions");
		}

		[Test]
		public void you_can_add_assembly_instructions_to_a_product() {
			var theImage = new UploadedFile(new FileInfo("res\\ponoko_logo_text_page.gif"), "image/gif");

			var theProduct = AssemblyInstructionRepository.Add(AnyProduct.Key, theImage);

			Assert.AreEqual(1, theProduct.AssemblyInstructions.Count, "Expected one assembly instructions file");
		}
	}

	public class UploadedFile {
		public FileInfo File { get; private set; }
		public String ContentType { get; private set; }
		public String FullName { get { return this.File.FullName; } }

		public UploadedFile(FileInfo file, String contentType) {
			File = file;
			ContentType = contentType;
		}
	}

	public class AssemblyInstructionRepository : Domain {
		public AssemblyInstructionRepository(TheInternet internet, String baseUrl) : base(internet, baseUrl) {}
		
		public Product Add(String productKey, params UploadedFile[] files) {
			var uri = Map("/products/{0}/assembly-instructions", productKey);

			var payload = ToPayload(files);

			using (var response = MultipartPost(uri, payload)) {
				return Deserialize(response);
			}
		}

		private Payload ToPayload(IEnumerable<UploadedFile> designImages) {
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

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Ponoko.Api.Core.Product;
using File = Ponoko.Api.Core.Product.File;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class ProductFileRepositoryAcceptanceTest : ProductAcceptanceTest {
		protected void AssertIncludesDesignImage(Product product, File file) {
			Assert.IsTrue(product.DesignImages.Exists(it => it.Filename == file.Filename), 
				"The design image <{0}> is not present", file.Filename
			);
		}

		protected void AssertIncludesAssemblyInstructions(Product product, File file) {
			Assert.IsTrue(product.AssemblyInstructions.Exists(it => it.Filename == file.Filename), 
				"The assembly instructions file <{0}> is not present", file.Filename
			);
		}

		protected String Checksum(Byte[] file) {
			var checksum = new MD5CryptoServiceProvider().ComputeHash(file);
			var buffer = new StringBuilder();

			for (var i = 0; i < checksum.Length; i++) {
				buffer.Append(checksum[i].ToString("x2"));
			}

			return buffer.ToString();
		}

		protected Byte[] ReadAll(Stream input) {
			const Int32 BUFFER_SIZE = 1024 * 10;

			using (var output = new MemoryStream()) {
				var buffer = new Byte[BUFFER_SIZE];
				var bytesRead = 0;
				var totalBytesRead = 0;
				while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
					output.Write(buffer, 0, bytesRead);
					totalBytesRead += bytesRead;
				}

				var result = new Byte[totalBytesRead];

				Buffer.BlockCopy(output.GetBuffer(), 0, result, 0, totalBytesRead);

				return result;
			}
		}
	}
}
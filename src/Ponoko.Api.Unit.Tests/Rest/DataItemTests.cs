using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest;

namespace Ponoko.Api.Unit.Tests.Rest {
	[TestFixture]
	public class DataItemTests {
		[Test] public void 
		filename_is_optional() {
			var expected = "name=\"example\"; filename=\"\"";
			var item = new DataItem("example", new byte[0], "");

			Assert.AreEqual(expected, item.AttributeString());
		}

		[Test] public void
		you_can_create_one_with_a_non_existent_file() {
			var item = new DataItem("example", new FileInfo("xxx_must_not_exist"), "text/plain");
			Assert.DoesNotThrow(() => item.AttributeString());
		}
	}
}

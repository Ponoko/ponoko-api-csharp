using NUnit.Framework;
using Ponoko.Api.Json.Generic;

namespace Ponoko.Api.Integration.Tests.Json
{
    [TestFixture]
    public class SimpleDeserializerTests
    {
        [Test]
        public void it_ignores_extra_fields()
        {
            var json =
                "{" +
                "	'name': 'Example name'," +
                "	'xxx_extra_field_xxx': 'Not suported on the Example class'" +
                "}";

            Assert.DoesNotThrow(() => SimpleDeserializer<Example>.Deserialize(json));
        }

        public class Example
        {
            public string Name;
        }
    }
}
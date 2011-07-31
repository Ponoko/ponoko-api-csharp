using System.IO;
using NUnit.Framework;
using Ponoko.Api.Rest.Mime;
using Rhino.Mocks;

namespace Ponoko.Api.Unit.Tests.Rest.Mime {
	[TestFixture]
	public class BodyTests {
		[Test]
		public void it_closes_its_underlying_stream() {
			var fakeStore = MockRepository.GenerateMock<Stream>();
			var body = new Body(fakeStore);

			body.Dispose();

			fakeStore.AssertWasCalled(it => it.Close(), options => options.
				Repeat.Once().
				Message("Expected that when a body is disposed of, it closes its underlying stream")
			);
		}
	}
}
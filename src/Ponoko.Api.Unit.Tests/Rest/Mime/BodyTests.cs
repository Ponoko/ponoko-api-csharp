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

		[Test]
		public void it_seeks_its_underlying_stream_to_its_start_when_open_is_invoked() {
			var fakeStore = MockRepository.GenerateMock<Stream>();
			var body = new Body(fakeStore);

			body.Open();

			fakeStore.AssertWasCalled(it => it.Seek(0, SeekOrigin.Begin), options => options.
				Repeat.Once().
				Message("Expected it to seek its underlying stream to its beginning")
			);
		}
	}
}
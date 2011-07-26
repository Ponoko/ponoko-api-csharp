using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Ponoko.Api.Json;
using Rhino.Mocks;

namespace Ponoko.Api.Integration.Tests.Json {
	[TestFixture]
	public class DateTimeReaderTests {
		[Test] 
		public void it_returns_utc_dates_that_include_the_time_of_day() {
			var jsonReader = MockRepository.GenerateStub<JsonReader>();
			var AmyWinehouseDiesAtAge27 = "2011/07/23 03:00:00 +0000";
			var reader = new DateTimeReader();

			jsonReader.Stub(it => it.Value).Return(AmyWinehouseDiesAtAge27);

			var expected = new DateTime(2011, 7, 23, 3, 0, 0, 0, DateTimeKind.Utc);
			var actual = reader.ReadJson(jsonReader, null, null, null);

			Assert.AreEqual(expected, actual);
		}
	}
}

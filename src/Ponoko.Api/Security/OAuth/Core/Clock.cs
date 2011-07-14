using System;
using OAuth.Net.Common;

namespace Ponoko.Api.Security.OAuth.Core {
    public interface Clock { String NewTimestamp(); }

    public class SystemClock : Clock {
        public string NewTimestamp() {
            return UnixTime.ToUnixTime(DateTime.Now).ToString();
        }
    }
}
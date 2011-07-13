using System;

namespace Ponoko.Net.Security.OAuth.Core {
    public class Options {
        public static Options Default = new Options("HMAC-SHA1", Decimal.Parse("1.0"));
        public String SignatureMethod { get; private set; }
        public Decimal Version { get; private set; }

        public Options(String signatureMethod, Decimal version) {
            SignatureMethod = signatureMethod;
            Version = version;
        }
    }
}
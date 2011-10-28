using System;

namespace Ponoko.Api.Rest.Security.OAuth.Core {
    public class Credential {
        public static readonly Credential Empty = new Credential(String.Empty, String.Empty);
        public string Key { get; private set; }
        public string Secret { get; private set; }

        public Credential(String key, String secret) {
            Key = key;
            Secret = secret;
        }
    }
}

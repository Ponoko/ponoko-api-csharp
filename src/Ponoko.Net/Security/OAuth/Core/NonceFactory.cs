using System;

namespace Ponoko.Net.Security.OAuth.Core {
    public interface NonceFactory { String NewNonce(); }

    public class SystemNonceFactory : NonceFactory {
        public string NewNonce() {
        	return Guid.NewGuid().ToString();
        }
    }
}
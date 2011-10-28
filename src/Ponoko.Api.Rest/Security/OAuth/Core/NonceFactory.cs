using System;

namespace Ponoko.Api.Rest.Security.OAuth.Core {
    public interface NonceFactory { String NewNonce(); }

    public class SystemNonceFactory : NonceFactory {
        public string NewNonce() {
        	return Guid.NewGuid().ToString();
        }
    }
}
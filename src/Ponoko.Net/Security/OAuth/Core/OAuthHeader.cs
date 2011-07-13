using System;

namespace Ponoko.Net.Security.OAuth.Core {
    public interface OAuthHeader {
        String New(Request request, CredentialSet credentials);
    }
}
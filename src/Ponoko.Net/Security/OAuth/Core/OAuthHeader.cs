using System;

namespace Ponoko.Api.Security.OAuth.Core {
    public interface OAuthHeader {
        String New(Request request, CredentialSet credentials);
    }
}
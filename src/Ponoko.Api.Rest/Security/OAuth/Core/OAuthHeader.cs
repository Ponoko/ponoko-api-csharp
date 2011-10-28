using System;

namespace Ponoko.Api.Rest.Security.OAuth.Core {
    public interface OAuthHeader {
        String New(Request request, CredentialSet credentials);
    }
}
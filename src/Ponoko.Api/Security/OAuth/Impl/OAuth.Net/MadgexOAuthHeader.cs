using System;
using OAuth.Net.Common;
using Ponoko.Api.Security.OAuth.Core;
using Ponoko.Api.Sugar;

namespace Ponoko.Api.Security.OAuth.Impl.OAuth.Net {
    public class MadgexOAuthHeader : OAuthHeader {
        private readonly Clock _clock;
        private readonly NonceFactory _nonceFactory;

        public MadgexOAuthHeader(Clock clock, NonceFactory nonceFactory) {
            _clock = clock;
            _nonceFactory = nonceFactory;
        }

        public string New(Request request, CredentialSet credentials) {
            var parameters = ToParameters(request, credentials);

            parameters.Signature = Sign(request, credentials, parameters);

            return parameters.ToHeaderFormat();
        }

        private String Sign(
            Request request,
            CredentialSet credentials,
            OAuthParameters parameters
        ) {
            return new MadgexSignature().Sign(
				request,
				credentials.Consumer.Secret, 
				credentials.Token.Secret, 
				parameters
			);
        }

		private OAuthParameters ToParameters(Request request, CredentialSet credentials) {
			var options = Options.Default;

			var parameters = new OAuthParameters {
				ConsumerKey     = credentials.Consumer.Key,
				Timestamp       = NewTimestamp(),
				Nonce           = NewNonce(),
				SignatureMethod = options.SignatureMethod,
				Version         = options.Version.ToString()
			};

			parameters.AdditionalParameters.Add(request.Payload.Parameters);

			un.less(() => String.IsNullOrEmpty(credentials.Token.Key), () => 
				parameters.Token = credentials.Token.Key
			);

			return parameters;
		}

        private String NewTimestamp() { return _clock.NewTimestamp(); }
        private String NewNonce() { return _nonceFactory.NewNonce(); }
    }
}
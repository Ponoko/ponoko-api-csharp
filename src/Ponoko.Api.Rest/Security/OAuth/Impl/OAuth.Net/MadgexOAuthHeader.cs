using System;
using System.Collections.Specialized;
using OAuth.Net.Common;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Sugar;

namespace Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net {
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
				Timestamp       = NewTimestamp,
				Nonce           = NewNonce,
				SignatureMethod = options.SignatureMethod,
				Version         = options.Version.ToString()
			};

			parameters.AdditionalParameters.Add(CollectAllParameters(request));

			un.less(String.IsNullOrEmpty(credentials.Token.Key), () => 
				parameters.Token = credentials.Token.Key
			);

			return parameters;
		}

		private NameValueCollection CollectAllParameters(Request from) {
			var result = new NameValueCollection();

			foreach (var parameter in from.RequestLine.Parameters) {
				result.Add(parameter.Name, parameter.Value.ToString());
			}
				
			foreach (var parameter in from.Payload) {
				result.Add(parameter.Name, parameter.Value != null ? parameter.Value.ToString() : null);
			}

			return result;
		}

        private String NewTimestamp { get { return _clock.NewTimestamp(); }}
        private String NewNonce { get { return _nonceFactory.NewNonce(); }}
    }
}
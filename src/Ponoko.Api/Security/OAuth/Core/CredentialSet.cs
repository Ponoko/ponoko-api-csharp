namespace Ponoko.Api.Security.OAuth.Core {
    public class CredentialSet {
        public Credential Consumer { get; private set; }
        public Credential Token { get; private set; }

        public CredentialSet(Credential consumer) : this(consumer, Credential.Empty) {}

        public CredentialSet(Credential consumer, Credential token) {
            Consumer = consumer;
            Token = token;
        }
    }
}

using System.Text.Json.Serialization;

namespace Quick.Keycloak.Client.Model
{
    public class KeyCloackAdapterOption
    {
        public string realm { get; set; }

        [JsonPropertyName("auth-server-url")]
        public string authserverurl { get; set; }

        [JsonPropertyName("ssl-required")]
        public string sslrequired { get; set; }
        public string resource { get; set; }

        [JsonPropertyName("verify-token-audience")]
        public bool verifytokenaudience { get; set; }
        public Credentials credentials { get; set; }

        [JsonPropertyName("confidential-port")]
        public int confidentialport { get; set; }

        [JsonPropertyName("policy-enforcer")]
        public PolicyEnforcer policyenforcer { get; set; }

    }

    public class Credentials
    {
        public string secret { get; set; }
    }

    public class PolicyEnforcer
    {
        public Credentials credentials { get; set; }
    }



}

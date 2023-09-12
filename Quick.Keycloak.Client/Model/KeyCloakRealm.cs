using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Quick.Keycloak.Client.Model
{
    /// <summary>
    /// get json from http://<keycloak>/realms/<realm>/
    /// </summary>
    internal class KeyCloakRealm
    {
        public string realm { get; set; }
        public string public_key { get; set; }

        /// <summary>
        /// symbol char '-'  error
        /// </summary>
        [JsonPropertyName("token-service")]
        public string tokenservice { get; set; }

        /// <summary>
        /// symbol char '-'  error
        /// </summary>
        [JsonPropertyName("account-service")]
        public string accountservice { get; set; }

        /// <summary>
        /// symbol char '-'  error
        /// </summary>
        [JsonPropertyName("tokens-not-before")]
        public int tokensnotbefore { get; set; }
    }
}

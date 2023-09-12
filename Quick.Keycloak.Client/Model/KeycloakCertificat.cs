using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Quick.Keycloak.Client.Model
{
    /// <summary>
    /// result json from request https://<keycloakserver>/realms/<realm>/protocol/openid-connect/certs
    /// </summary>
    public class KeycloakPublicKeys
    {
        public List<Key> keys { get; set; }
    }

    public class Key
    {
        public string kid { get; set; }
        public string kty { get; set; }
        public string alg { get; set; }
        public string use { get; set; }
        public string n { get; set; }
        public string e { get; set; }
        public List<string> x5c { get; set; }
        public string x5t { get; set; }

        /// <summary>
        /// symbol char '#'  error
        /// </summary>
        [JsonPropertyName("x5t#S256")]
        public string x5tS256 { get; set; }
    }
}

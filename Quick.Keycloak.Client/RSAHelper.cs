using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Quick.Keycloak.Client.Model;

namespace Quick.Keycloak.Client
{
    internal class RSAHelper
    {
        internal static async Task<RsaSecurityKey?> GetRSAKeyAsync(string urlIssuer, string realm)
        {
            string publicKeyJWT = await RSAPublicKeyAsync(urlIssuer, realm);
            if (string.IsNullOrEmpty(publicKeyJWT))
                return null;
            RsaSecurityKey rsa = BuildRSAKey(publicKeyJWT);
            return rsa;
        }

        /// <summary>
        /// build RSA Security Key from public JWT key
        /// </summary>
        /// <param name="publicKeyJWT"></param>
        /// <returns></returns>
        private static RsaSecurityKey BuildRSAKey(string publicKeyJWT)
        {
            RSA rsa = RSA.Create();

            rsa.ImportSubjectPublicKeyInfo(

                source: Convert.FromBase64String(publicKeyJWT),
                bytesRead: out _
            );

            var IssuerSigningKey = new RsaSecurityKey(rsa);

            return IssuerSigningKey;
        }

        /// <summary>
        /// Retrieve PublicKey from Keycloack
        /// </summary>
        /// <param name="urlIssuer">url keycloack auh</param>
        /// <param name="realm">realm</param>
        /// <returns>public key string </returns>
        private static async Task<string> RSAPublicKeyAsync(string urlIssuer, string realm)
        {
            string publicKey = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(urlIssuer);
                    client.Timeout = new TimeSpan(0, 0, 10);
                    var response = await client.GetAsync($"/realms/{realm}");

                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        var realmInfo = JsonConvert.DeserializeObject<KeyCloakRealm>(data);
                        publicKey = realmInfo.public_key;
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.Message);
            }

            return publicKey;
        }
    }
}

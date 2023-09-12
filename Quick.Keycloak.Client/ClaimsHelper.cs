using Newtonsoft.Json;
using Quick.Keycloak.Client.Model.claim;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Quick.Keycloak.Client
{
    public static class ClaimsHelper
    {
        const string RESOURCE_ACCESS = "resource_access";
        const string REALM_ACCESS = "realm_access";
        public static ClaimsPrincipal Transform(this ClaimsPrincipal principal, Action<ClaimsIdentity> customTransformClaim)
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)principal.Identity;

            if (claimsIdentity != null)
            {
                if (customTransformClaim != null)
                    // custom read claim
                    customTransformClaim(claimsIdentity);
                else
                {
                    // Keycloack Roles from Ressource and Realm Access flat Claims
                    if (claimsIdentity.IsAuthenticated && claimsIdentity.HasClaim((claim) => claim.Type == RESOURCE_ACCESS))
                    {
                        var userRoleRessource = claimsIdentity.FindFirst((claim) => claim.Type == RESOURCE_ACCESS);
                        if (userRoleRessource.Value != null)
                        {
                            var ressource = JsonConvert.DeserializeObject<Ressource>(userRoleRessource.Value);

                            if (ressource != null)
                                foreach (var role in ressource.account.roles)
                                {
                                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));

                                }
                        }
                    }

                    if (claimsIdentity.IsAuthenticated && claimsIdentity.HasClaim((claim) => claim.Type == REALM_ACCESS))
                    {
                        var userRoleRealm = claimsIdentity.FindFirst((claim) => claim.Type == REALM_ACCESS);
                        if (userRoleRealm.Value != null)
                        {
                            var realmAccess = JsonConvert.DeserializeObject<RealmAccess>(userRoleRealm.Value);

                            if (realmAccess != null)
                                foreach (var role in realmAccess.roles)
                                {
                                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                                }
                        }
                    }
                }
            }
            return principal;
        }
    }
}

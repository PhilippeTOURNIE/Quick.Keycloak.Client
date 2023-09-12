
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Quick.Keycloak.Client.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Quick.Keycloak.Client
{
    public static class KeycloackJwtHelper
    {
        
        /// <summary>
        /// Add Keycloak Security width token 
        /// use data adatpter from appsettings file
        /// </summary>
        /// <param name="services">service</param>
        /// <param name="configuration">Use appsettings keyword "KeycloackAdatpter" </param>
        public static void AddQuickKeycloakTokenJWT(this WebApplicationBuilder builder, Action<ClaimsIdentity> transformClaim =null)
        {
            var keyCloackAdapterOption = new KeyCloackAdapterOption();
            builder.Configuration.GetSection("KeycloackAdatpter").Bind(keyCloackAdapterOption);
            var section = builder.Configuration.GetSection("KeycloackAdatpter");
            keyCloackAdapterOption.authserverurl =  section.GetValue<string>("auth-server-url");
            keyCloackAdapterOption.sslrequired = section.GetValue<string>("ssl-required");
            keyCloackAdapterOption.verifytokenaudience = section.GetValue<bool>("verify-token-audience");
            keyCloackAdapterOption.confidentialport = section.GetValue<int>("confidential-port");
            keyCloackAdapterOption.policyenforcer = section.GetValue<PolicyEnforcer>("policy-enforcer");
            builder.AddQuickKeycloakTokenJWT(keyCloackAdapterOption,transformClaim);
        }

        public static void AddQuickKeycloakTokenJWT(this WebApplicationBuilder builder, KeyCloackAdapterOption keyCloackAdapterOption, Action<ClaimsIdentity> transformClaim = null)
        {
             
            if (string.IsNullOrEmpty(keyCloackAdapterOption.authserverurl))
            {
                // log no settings
            }

            // JWT Schema authentication
            var AuthenticationBuilder = builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            AuthenticationBuilder.AddJwtBearer(async o =>
            {

                // JWT Token validation
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuers = new[] {  $"{keyCloackAdapterOption.authserverurl}realms/{keyCloackAdapterOption.realm}" }, 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = await RSAHelper.GetRSAKeyAsync(keyCloackAdapterOption.authserverurl, keyCloackAdapterOption.realm),
                    ValidateLifetime = true                };

                // Event Authentification Handlers

                o.Events = new JwtBearerEvents()
                {
                    // On first user demand 
                    //OnMessageReceived = async mr =>
                    //{
                    //    // we get last cert from keycloak
                    //    o.TokenValidationParameters.IssuerSigningKey =
                    //     await RSAHelper.GetRSAKeyAsync(keyCloackAdapterOption.authserverurl, keyCloackAdapterOption.realm);
                    //},
                    // on validation
                    OnTokenValidated = c =>
                    {
                        c.Principal.Transform(transformClaim);
                        System.Diagnostics.Debug.Print("User successfully authenticated");
                        return Task.CompletedTask;
                    },
                    // on failed
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();

                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";

                        if (builder.Environment.IsDevelopment())
                        {
                            return c.Response.WriteAsync(c.Exception.ToString());
                        }
                        return c.Response.WriteAsync("An error occured processing your authentication.");
                    }
                };

            });
        }
    }
}
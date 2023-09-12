using Newtonsoft.Json;
using Quick.Keycloak.Client;
using Quick.Keycloak.Client.Model.claim;
using Quick.SwaggerWidthApiVersion;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddQuickSwaggerWidthApiVersion("Demo", "v1");

builder.Services.AddControllers();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "RealAdmin"));
});

builder.AddQuickKeycloakTokenJWT();

//builder.AddQuickKeycloakTokenJWT(claim =>
//{
//    var userRoleRealm = claim.FindFirst((claim) => claim.Type == "realm_access");
//    if (userRoleRealm != null)
//    {
//        var realmAccess = JsonConvert.DeserializeObject<RealmAccess>(userRoleRealm.Value);

//        if (realmAccess != null)
//            foreach (var role in realmAccess.roles)
//            {
//                claim.AddClaim(new Claim(ClaimTypes.Role, role));
//            }
//    }
//});

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.AddQuickUseSwagger();

app.Run();

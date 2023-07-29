using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CorporateSoftware.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles",
                "Your role(s)",
                new [] { "role" })

        };
    public static IEnumerable<ApiResource> ApiResource =>
        new ApiResource[]
            {
                new ApiResource("todolistapi", "Todo List API")
                {
                    Scopes = { "todolistapi.fullaccess"}
                }
            };
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            {
                new ApiScope("todolistapi.fullaccess")
            };

    public static IEnumerable<Client> Clients =>
        new Client[] 
            {
               new Client()
               {
                   ClientName = "Todo List",
                   ClientId = "todolistclient",
                   AllowedGrantTypes= GrantTypes.Code,
                   RedirectUris =
                   {
                       "https://localhost:7255/signin-oidc"
                   },
                   PostLogoutRedirectUris =
                   {
                       "https://localhost:7255/signout-callback-oidc"
                   },
                   AllowedScopes =
                   {
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       "roles",
                       "todolistapi.fullaccess"
                   },
                   ClientSecrets =
                   {
                       new Secret("secret".Sha256())
                   },
                   RequireConsent = false
               }
            };
}
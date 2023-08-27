using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    /// <summary>
    /// Group of claims about a user that can be requested using scoped param
    /// </summary>
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            // this is the required scope the add the subjectId into identity token (WTF?)
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new(name: "myapi", displayName: "MyFirstApi"),
            new(name: "myapi2", displayName: "MySecondApi")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new ()
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                
                AllowedScopes = { "myapi" ,"myapi2"}
            },
            // interactive ASP.NET Core Web App
            new ()
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
            
                // where to redirect to after login
                RedirectUris = { "https://fakewebclient.qapitacorp.local/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://fakewebclient.qapitacorp.local/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            }
        };
}
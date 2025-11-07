using Duende.IdentityServer.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("auctionApp", "Auction app full access"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman",
                    AllowedScopes = {"openid", "profile", "auctionApp"},
                    RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                    ClientSecrets = new[] {new Secret("NotASecret".Sha256())},
                    AllowedGrantTypes = {GrantType.ResourceOwnerPassword}
                 },
                new Client
                {
                    ClientId = "angularApp",
                    ClientName = "angularApp",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {"http://localhost:4200"},
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200"},
                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "auctionApp"},
                    RequireConsent = false,
                    FrontChannelLogoutUri = "http://localhost:4200/logout",
                    FrontChannelLogoutSessionRequired = true,
                    AccessTokenLifetime = 3600,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
    }
}

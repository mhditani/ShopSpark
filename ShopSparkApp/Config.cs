using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace ShopSparkApp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "Your roles", new[] { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("shopspark.api", "ShopSpark Full API Access"),
                new ApiScope("products.read", "Read products"),
                new ApiScope("products.write", "Write products"),
                new ApiScope("products.delete", "Delete products"),
                new ApiScope("orders.read", "Read orders"),
                new ApiScope("orders.write", "Create and update orders"),
                new ApiScope("orders.manage", "Manage all orders"),
                new ApiScope("customers.read", "Read customer data"),
                new ApiScope("customers.manage", "Manage customer accounts"),
                new ApiScope("inventory", "Inventory management"),
                new ApiScope("reports", "Access reports and analytics")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                        ClientId = "shopspark-password",
            ClientName = "ShopSpark Password Client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // This allows password grant
            RequireClientSecret = true,
            ClientSecrets = { new Secret("password-secret".Sha256()) },
            AllowedScopes =
            {
                "openid",
                "profile",
                "email",
                "roles",
                "shopspark.api",
                "products.read",
                "products.write",
                "orders.read",
                "orders.write",
                "customers.read"
            },
            AlwaysIncludeUserClaimsInIdToken = true,
            AllowOfflineAccess = true,
            AccessTokenLifetime = 3600
                },

                // Machine-to-Machine Client (API-to-API communication)
                new Client
                {
                    
                    ClientId = "shopspark-api",
                    ClientName = "ShopSpark API Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = true,
                    ClientSecrets = { new Secret("api-secret".Sha256()) },

                    AllowedScopes =
                    {
                        "shopspark.api",
                        "products.read",
                        "products.write",
                        "orders.manage",
                        "inventory",
                        "reports"
                    },

                    AccessTokenLifetime = 3600
                },

                // Future Frontend Client (if you add one later)
                new Client
                {
                    ClientId = "shopspark-frontend",
                    ClientName = "ShopSpark Frontend",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    ClientSecrets = { new Secret("frontend-secret".Sha256()) },

                    RedirectUris = { "https://localhost:3000/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:3000/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "email",
                        "roles",
                        "shopspark.api",
                        "products.read",
                        "orders.read",
                        "orders.write",
                        "customers.read"
                    },

                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false
                }
            };

        public static List<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin@shopspark.com",
                    Password = "Admin123!",
                    Claims = new List<Claim>
                    {
                        new Claim("role", "Admin"),
                        new Claim("role", "Manager"),
                        new Claim("given_name", "Admin"),
                        new Claim("family_name", "User"),
                        new Claim("email", "admin@shopspark.com"),
                        new Claim("email_verified", "true")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "customer@shopspark.com",
                    Password = "Customer123!",
                    Claims = new List<Claim>
                    {
                        new Claim("role", "Customer"),
                        new Claim("given_name", "John"),
                        new Claim("family_name", "Doe"),
                        new Claim("email", "customer@shopspark.com"),
                        new Claim("email_verified", "true")
                    }
                }
            };
    }
}
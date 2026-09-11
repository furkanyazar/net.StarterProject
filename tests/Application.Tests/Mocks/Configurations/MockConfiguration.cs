using Microsoft.Extensions.Configuration;

namespace Application.Tests.Mocks.Configurations;

public static class MockConfiguration
{
    public static IConfiguration GetConfigurationMock()
    {
        IEnumerable<KeyValuePair<string, string?>> mockConfiguration = new Dictionary<
            string,
            string?
        >
        {
            { "TokenOptions:Audience", "net.StarterProject" },
            { "TokenOptions:Issuer", "net" },
            { "TokenOptions:AccessTokenExpiration", "10" },
            {
                "TokenOptions:SecurityKey",
                "StrongAndSecretKeyStrongAndSecretKeyStrongAndSecretKeyStrongAndSecretKey"
            },
            { "TokenOptions:RefreshTokenTTL", "7" },
        };
        IConfigurationBuilder configuration = new ConfigurationBuilder().AddInMemoryCollection(
            mockConfiguration
        );
        return configuration.Build();
    }
}

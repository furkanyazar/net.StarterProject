using Application.Features.Auth.Commands.Login;
using Application.Tests.Mocks.FakeDatas;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests.DependencyResolvers;

public static class AuthServiceRegistrations
{
    public static void AddAuthServices(this IServiceCollection services)
    {
        services.AddTransient<UserFakeData>();
        services.AddTransient<LoginCommand>();
    }
}

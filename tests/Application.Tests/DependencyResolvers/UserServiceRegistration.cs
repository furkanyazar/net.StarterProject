using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Tests.Mocks.FakeData;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests.DependencyResolvers;

public static class UserServiceRegistration
{
    public static void AddUserServices(this IServiceCollection services)
    {
        services.AddTransient<UserFakeData>();
        services.AddTransient<RegisterCommand>();
        services.AddTransient<LoginCommand>();
        services.AddTransient<RefreshTokenCommand>();
        services.AddTransient<RevokeTokenCommand>();
        services.AddTransient<ForgotPasswordCommand>();
        services.AddTransient<ResetPasswordCommand>();
        services.AddSingleton<RegisterCommandValidator>();
        services.AddSingleton<LoginCommandValidator>();
        services.AddSingleton<ForgotPasswordCommandValidator>();
        services.AddSingleton<ResetPasswordCommandValidator>();
    }
}

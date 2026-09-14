using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.RevokeToken;
using Application.Tests.Mocks.FakeDatas;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests.DependencyResolvers;

public static class AuthServiceRegistrations
{
    public static void AddAuthServices(this IServiceCollection services)
    {
        services.AddTransient<UserFakeData>();
        services.AddTransient<EmailAuthenticatorFakeData>();
        services.AddTransient<RefreshTokenFakeData>();

        services.AddTransient<LoginCommand>();
        services.AddTransient<LoginCommandValidator>();

        services.AddTransient<RegisterCommand>();
        services.AddTransient<RegisterCommandValidator>();

        services.AddTransient<ForgotPasswordCommand>();
        services.AddTransient<ForgotPasswordCommandValidator>();

        services.AddTransient<ResetPasswordCommand>();
        services.AddTransient<ResetPasswordCommandValidator>();

        services.AddTransient<RefreshTokenCommand>();

        services.AddTransient<RevokeTokenCommand>();
    }
}

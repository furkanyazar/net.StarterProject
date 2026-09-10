using Application.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string baseDbConnectionStringName = "BaseDb";
        string connectionString =
            configuration.GetConnectionString(baseDbConnectionStringName)
            ?? throw new InvalidOperationException(
                $"\"{baseDbConnectionStringName}\" connection string cannot found in configuration."
            );

        services.AddDbContext<BaseDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();
        services.AddScoped<IUserGroupOperationClaimRepository, UserGroupOperationClaimRepository>();

        return services;
    }
}

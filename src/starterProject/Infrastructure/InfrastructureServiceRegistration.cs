using Application.Services.MailTemplateService;
using Application.Services.QueueService;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Infrastructure.Adapters.MailTemplateService;
using Infrastructure.Adapters.QueueService;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<MailTemplateServiceBase, FluidMailTemplateServiceAdapter>();

        services.AddSingleton<QueueServiceBase, RabbitMQQueueServiceAdapter>(provider =>
            RabbitMQQueueServiceAdapter
                .CreateAsync(
                    provider.GetRequiredService<IConfiguration>(),
                    provider.GetRequiredService<ILogger>()
                )
                .GetAwaiter()
                .GetResult()
        );

        services.AddHostedService<QueueHostedService>();

        return services;
    }
}

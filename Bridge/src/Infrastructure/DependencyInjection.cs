namespace Bridge.Infrastructure;

using Bridge.Domain.Interfaces;
using Bridge.Infrastructure.Configuration;
using Bridge.Infrastructure.OpcUa;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<OpcUaClientOptions>()
            .BindConfiguration(OpcUaClientOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IOpcUaClient, OpcUaClient>();

        return services;
    }
}

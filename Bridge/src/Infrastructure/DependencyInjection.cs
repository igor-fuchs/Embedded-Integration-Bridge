namespace Bridge.Infrastructure;

using Bridge.Domain.Interfaces;
using Bridge.Infrastructure.Configuration;
using Bridge.Infrastructure.Http.Client;
using Bridge.Infrastructure.OpcUa.Client;
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
        // OPC UA Client
        services.AddOptions<OpcUaClientOptions>()
            .BindConfiguration(OpcUaClientOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IOpcUaClient, OpcUaClient>();

        // API Client
        services.AddOptions<ApiClientOptions>()
            .BindConfiguration(ApiClientOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiClientOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}

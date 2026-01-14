namespace Bridge.Application;

using Bridge.Application.Handlers;
using Bridge.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Handlers
        services.AddSingleton<INodeValueChangedHandler, NodeValueChangedHandler>();

        return services;
    }
}

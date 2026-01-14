namespace Bridge.Infrastructure.Caching;

using Bridge.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

/// <summary>
/// In-memory cache for tracking registered nodes in the API.
/// Thread-safe implementation using ConcurrentDictionary.
/// </summary>
public sealed class RegisteredNodeCache : IRegisteredNodeCache
{
    private readonly IApiClient _apiClient;
    private readonly ILogger<RegisteredNodeCache> _logger;
    private readonly ConcurrentDictionary<string, bool> _cache = new();

    public RegisteredNodeCache(
        IApiClient apiClient,
        ILogger<RegisteredNodeCache> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsRegistered(string nodeName) =>
        _cache.ContainsKey(nodeName);

    /// <inheritdoc />
    public void Register(string nodeName) =>
        _cache[nodeName] = true;

    /// <inheritdoc />
    public async Task LoadFromApiAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔄 Loading registered nodes from API...");

        var result = await _apiClient.GetRegisteredNodesAsync(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "⚠️ Failed to load registered nodes: {Error}. All nodes will be created as new.",
                result.Error.Description);
            return;
        }

        foreach (var node in result.Value.Nodes)
        {
            _cache[node.Name] = true;
        }

        _logger.LogInformation("✅ Loaded {Count} registered nodes", result.Value.TotalCount);
    }
}

namespace Bridge.Application.Services;

using Bridge.Domain.DTOs;
using Bridge.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background service responsible for orchestrating OPC UA node monitoring.
/// Handles lifecycle, connection, subscription, and delegates value change processing.
/// </summary>
public sealed class OpcUaMonitoringService : BackgroundService
{
    private readonly IOpcUaClient _opcUaClient;
    private readonly IApiClient _apiClient;
    private readonly IRegisteredNodeCache _nodeCache;
    private readonly INodeValueChangedHandler _valueChangedHandler;
    private readonly ILogger<OpcUaMonitoringService> _logger;

    public OpcUaMonitoringService(
        IOpcUaClient opcUaClient,
        IApiClient apiClient,
        IRegisteredNodeCache nodeCache,
        INodeValueChangedHandler valueChangedHandler,
        ILogger<OpcUaMonitoringService> logger)
    {
        _opcUaClient = opcUaClient;
        _apiClient = apiClient;
        _nodeCache = nodeCache;
        _valueChangedHandler = valueChangedHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OPC UA Monitoring Service is starting...");

        try
        {
            // Load registered nodes from API to know which nodes already exist
            await _nodeCache.LoadFromApiAsync(stoppingToken);

            // Fetch node names to monitor from API
            var nodeNamesToMonitor = await GetNodeNamesToMonitorAsync(stoppingToken);

            if (nodeNamesToMonitor.Count == 0)
            {
                _logger.LogWarning("⚠️ No node names found to monitor. Service will wait for cancellation.");
                await Task.Delay(Timeout.Infinite, stoppingToken);
                return;
            }

            await _opcUaClient.ConnectAsync(stoppingToken);
            _logger.LogInformation("Connected to OPC UA server");

            await _opcUaClient.SubscribeAsync(
                nodeNamesToMonitor,
                OnValueChanged,
                stoppingToken);

            _logger.LogInformation("📡 Monitoring {Count} node(s) for changes", nodeNamesToMonitor.Count);

            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OPC UA Monitoring Service is stopping...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OPC UA Monitoring Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disconnecting from OPC UA server...");
        await _opcUaClient.DisconnectAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Fetches the node names to monitor from the API.
    /// </summary>
    private async Task<IReadOnlyList<string>> GetNodeNamesToMonitorAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Fetching node names to monitor from API...");

        var result = await _apiClient.GetNodeNamesAsync(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError("❌ Failed to fetch node names: {Error}", result.Error.Description);
            return [];
        }

        var nodeNames = result.Value.Nodes
            .Select(n => n.Name)
            .ToList();

        _logger.LogInformation("✅ Retrieved {Count} node names to monitor", nodeNames.Count);

        return nodeNames;
    }

    /// <summary>
    /// Handles value changes from OPC UA nodes by delegating to the handler.
    /// </summary>
    /// <remarks>
    /// This is async void because it's an event handler callback.
    /// All exceptions are caught and logged to prevent crashes.
    /// </remarks>
    private async void OnValueChanged(NodeDTO node)
    {
        try
        {
            await _valueChangedHandler.HandleAsync(node);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing node change for {NodeId}", node.Name);
        }
    }
}

namespace Bridge.Application.Services;

using Bridge.Domain.Interfaces;
using Bridge.Domain.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bridge.Domain.DTOs;

/// <summary>
/// Background service responsible for monitoring OPC UA nodes.
/// </summary>
public sealed class OpcUaMonitoringService : BackgroundService
{
    private readonly IOpcUaClient _opcUaClient;
    private readonly IApiClient _apiClient;
    private readonly ILogger<OpcUaMonitoringService> _logger;

    public OpcUaMonitoringService(
        IOpcUaClient opcUaClient,
        IApiClient apiClient,
        ILogger<OpcUaMonitoringService> logger)
    {
        _opcUaClient = opcUaClient;
        _apiClient = apiClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OPC UA Monitoring Service is starting...");

        try
        {
            // Load existing nodes from API
            await LoadExistingNodesAsync(stoppingToken);

            await _opcUaClient.ConnectAsync(stoppingToken);
            _logger.LogInformation("Connected to OPC UA server");

            await _opcUaClient.SubscribeAsync(
                OpcNodes.NodeIdsToMonitor,
                OnValueChanged,
                stoppingToken);

            _logger.LogInformation("📡 Monitoring {Count} node(s) for changes", OpcNodes.NodeIdsToMonitor.Count);

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
    /// Loads existing nodes from the API to populate the known nodes cache.
    /// </summary>
    private async Task LoadExistingNodesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Loading existing nodes from API...");

        var result = await _apiClient.GetRegisteredNodesAsync(cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation("✅ Loaded {Count} existing nodes", result.Value.TotalCount);
        }
        else
        {
            _logger.LogWarning("⚠️ Failed to load existing nodes: {Error}", result.Error.Description);
        }
    }

    /// <summary>
    /// Handles value changes from OPC UA nodes.
    /// Note: This is async void because it's an event handler callback.
    /// All exceptions are caught and logged to prevent crashes.
    /// </summary>
    private async void OnValueChanged(NodeDTO node)
    {
        _logger.LogDebug("{NodeId} => {Value}", node.Name, node.Value);
    }
}

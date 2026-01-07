namespace Bridge.Application.Services;

using Bridge.Domain.Interfaces;
using Bridge.Domain.Constants;
using Bridge.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

    private async void OnValueChanged(OpcNodeValue nodeValue)
    {
        _logger.LogInformation("{Timestamp:HH:mm:ss} | {NodeId} => {Value}",
            nodeValue.Timestamp, nodeValue.NodeId, nodeValue.Value);

        var success = await _apiClient.SendNodeValueAsync(nodeValue);
        
        if (success)
        {
            _logger.LogInformation("✅ Data sent to API for node {NodeId}", nodeValue.NodeId);
        }
        else
        {
            _logger.LogWarning("❌ Failed to send data to API for node {NodeId}", nodeValue.NodeId);
        }
    }
}

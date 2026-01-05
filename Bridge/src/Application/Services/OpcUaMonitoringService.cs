namespace Bridge.Application.Services;

using Bridge.Domain.Abstractions;
using Bridge.Domain.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background service responsible for monitoring OPC UA nodes.
/// </summary>
public sealed class OpcUaMonitoringService : BackgroundService
{
    private readonly IOpcUaClient _opcUaClient;
    private readonly ILogger<OpcUaMonitoringService> _logger;

    public OpcUaMonitoringService(
        IOpcUaClient opcUaClient,
        ILogger<OpcUaMonitoringService> logger)
    {
        _opcUaClient = opcUaClient;
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

    private void OnValueChanged(string nodeId, object? value, DateTime timestamp)
    {
        _logger.LogInformation("{Timestamp:HH:mm:ss} | {NodeId} => {Value}",
            timestamp, nodeId, value);
    }
}

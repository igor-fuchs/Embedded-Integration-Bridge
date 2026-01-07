namespace Bridge.Application.Services;

using Bridge.Domain.Interfaces;
using Bridge.Domain.Constants;
using Bridge.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

/// <summary>
/// Background service responsible for monitoring OPC UA nodes.
/// </summary>
public sealed class OpcUaMonitoringService : BackgroundService
{
    private readonly IOpcUaClient _opcUaClient;
    private readonly IApiClient _apiClient;
    private readonly ILogger<OpcUaMonitoringService> _logger;
    private readonly ConcurrentDictionary<string, bool> _createdNodes = new();

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
    /// Loads existing nodes from the API to populate the created nodes cache.
    /// </summary>
    private async Task LoadExistingNodesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Loading existing nodes from API...");

        var response = await _apiClient.GetAllNodesAsync(cancellationToken);

        if (response?.Nodes is not null)
        {
            foreach (var node in response.Nodes)
            {
                _createdNodes[node.Name] = true;
            }

            _logger.LogInformation("✅ Loaded {Count} existing node(s) from API", response.TotalCount);
        }
        else
        {
            _logger.LogWarning("⚠️ Could not load existing nodes from API. Will create nodes as needed.");
        }
    }

    private async void OnValueChanged(OpcNodeValue nodeValue)
    {
        _logger.LogInformation("{Timestamp:HH:mm:ss} | {NodeId} => {Value}",
            nodeValue.Timestamp, nodeValue.NodeId, nodeValue.Value);

        bool success;

        // Check if node was already created in this session
        if (_createdNodes.TryGetValue(nodeValue.NodeId, out var isCreated) && isCreated)
        {
            // Node already exists, update it
            await _apiClient.UpdateNodeAsync(nodeValue);
    
        }
        else
        {
            // Try to create the node first
            success = await _apiClient.CreateNodeAsync(nodeValue);
            
            if (success)
            {
                _createdNodes[nodeValue.NodeId] = true;
            }
            else
            {
                // Creation failed, try to update (node might already exist in the server)
                success = await _apiClient.UpdateNodeAsync(nodeValue);
                
                if (success)
                {
                    _createdNodes[nodeValue.NodeId] = true;
                }
            }
        }
    }
}

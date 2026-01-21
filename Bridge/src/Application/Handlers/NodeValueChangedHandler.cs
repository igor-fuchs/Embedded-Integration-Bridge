namespace Bridge.Application.Handlers;

using Bridge.Domain.DTOs;
using Bridge.Domain.DTOs.Requests;
using Bridge.Domain.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles OPC UA node value changes by creating or updating nodes in the API.
/// </summary>
public sealed class NodeValueChangedHandler : INodeValueChangedHandler
{
    private readonly IApiClient _apiClient;
    private readonly IRegisteredNodeCache _nodeCache;
    private readonly ILogger<NodeValueChangedHandler> _logger;

    public NodeValueChangedHandler(
        IApiClient apiClient,
        IRegisteredNodeCache nodeCache,
        ILogger<NodeValueChangedHandler> logger)
    {
        _apiClient = apiClient;
        _nodeCache = nodeCache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(NodeDTO node)
    {
        _logger.LogDebug("{NodeId} => {Value}", node.Name, node.Value);

        if (_nodeCache.IsRegistered(node.Name))
        {
            await UpdateNodeAsync(node);
        }
        else
        {
            await CreateNodeAsync(node);
        }
    }

    private async Task CreateNodeAsync(NodeDTO node)
    {
        var request = new CreateNodeRequest
        {
            Name = node.Name,
            Value = node.Value
        };

        var result = await _apiClient.CreateNodeAsync(request);

        if (result.IsSuccess)
        {
            _nodeCache.Register(node.Name);
            _logger.LogDebug("✅ Node created: {NodeId}", node.Name);
        }
        else
        {
            _logger.LogWarning("❌ Failed to create node {NodeId}: {Error}", node.Name, result.Error.Description);
        }
    }

    private async Task UpdateNodeAsync(NodeDTO node)
    {
        var request = new UpdateNodeRequest
        {
            Name = node.Name,
            Value = node.Value
        };

        var result = await _apiClient.UpdateNodeAsync(request);

        if (result.IsFailure)
        {
            _logger.LogWarning("❌ Failed to update node {NodeId}: {Error}", node.Name, result.Error.Description);
        }
    }
}

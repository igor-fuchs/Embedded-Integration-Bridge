namespace Bridge.Infrastructure.Http.Client;

using Bridge.Domain.Common;
using Bridge.Domain.DTOs.Requests;
using Bridge.Domain.DTOs.Responses;
using Bridge.Domain.Interfaces;
using Bridge.Infrastructure.Configuration;
using Bridge.Infrastructure.Http.Abstractions;
using Bridge.Infrastructure.Http.Endpoints;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// HTTP client for sending OPC UA node values to the API.
/// </summary>
public sealed class ApiClient : IApiClient
{
    private readonly IHttpClientWrapper _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(
        HttpClient httpClient,
        ILogger<ApiClient> logger,
        IOptions<ApiClientOptions> options)
    {
        _httpClient = new HttpClientWrapper(
            httpClient,
            logger,
            options.Value.BaseUrl);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> CreateNodeAsync(CreateNodeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.PostAsync(
            ApiEndpoints.OpcUaNodes.Create,
            request,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogDebug("Node created successfully: {NodeName}", request.Name);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result> UpdateNodeAsync(UpdateNodeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.PutAsync(
            ApiEndpoints.OpcUaNodes.Update(request.Name),
            request,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogDebug("Node updated successfully: {NodeName}", request.Name);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<RegisteredNodesResponse>> GetRegisteredNodesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync<RegisteredNodesResponse>(
            ApiEndpoints.OpcUaNodes.GetRegisteredNodes,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogDebug("Retrieved {Count} registered nodes", result.Value.TotalCount);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<Result<NodeNamesResponse>> GetNodeNamesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetAsync<NodeNamesResponse>(
            ApiEndpoints.OpcUaNodes.GetNames,
            cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogDebug("Retrieved {Count} node names", result.Value.TotalCount);
        }

        return result;
    }
}

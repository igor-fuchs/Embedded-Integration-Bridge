namespace Bridge.Infrastructure.Http;

using Bridge.Domain.Interfaces;
using Bridge.Domain.Models;
using Bridge.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

/// <summary>
/// HTTP client for sending OPC UA node values to the API.
/// </summary>
public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly ApiClientOptions _options;

    public ApiClient(
        HttpClient httpClient,
        ILogger<ApiClient> logger,
        IOptions<ApiClientOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<bool> CreateNodeAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new
            {
                name = nodeValue.NodeId,
                value = nodeValue.Value
            };

            var url = $"{_options.BaseUrl}{_options.OpcUaNodesEndpoint}";

            var response = await _httpClient.PostAsJsonAsync(url, body, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ Node created: {NodeId}", nodeValue.NodeId);
                return true;
            }

            _logger.LogWarning(
                "❌ Failed to send node value to API. NodeId: {NodeId}\n   - StatusCode: {StatusCode}\n   - Response: {Response}",
                nodeValue.NodeId,
                response.StatusCode,
                response.Content.ReadAsStringAsync(cancellationToken).Result);

            return false;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("⏸️ Request cancelled for node {NodeId}", nodeValue.NodeId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending node value to API: {NodeId}", nodeValue.NodeId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateNodeAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new
            {
                value = nodeValue.Value
            };

            var url = $"{_options.BaseUrl}{_options.OpcUaNodesEndpoint}/{nodeValue.NodeId}";

            var response = await _httpClient.PutAsJsonAsync(url, body, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ Node updated: {NodeId}", nodeValue.NodeId);
                return true;
            }

            _logger.LogWarning(
                "❌ Failed to update node value to API. NodeId: {NodeId}\n   - StatusCode: {StatusCode}\n   - Response: {Response}",
                nodeValue.NodeId,
                response.StatusCode,
                response.Content.ReadAsStringAsync(cancellationToken).Result);

            return false;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("⏸️ Request cancelled for node {NodeId}", nodeValue.NodeId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending node value to API: {NodeId}", nodeValue.NodeId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<ApiNodesResponse?> GetAllNodesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_options.BaseUrl}{_options.OpcUaNodesEndpoint}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiNodesResponse>(cancellationToken);
                _logger.LogDebug("✅ Nodes retrieved successfully. Count: {Count}", result?.TotalCount ?? 0);
                return result;
            }

            _logger.LogWarning(
                "❌ Failed to retrieve nodes\n   - StatusCode: {StatusCode}\n   - Response: {Response}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));

            return null;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("⏸️ Request cancelled for retrieving nodes");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving nodes from API");
            return null;
        }
    }
}   

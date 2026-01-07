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
    public async Task<bool> SendNodeValueAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default)
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
                _logger.LogDebug("Successfully sent node value to API: {NodeId}", nodeValue.NodeId);
                return true;
            }

            _logger.LogWarning(
                "Failed to send node value to API. NodeId: {NodeId}\n   - StatusCode: {StatusCode}\n   - Response: {Response}",
                nodeValue.NodeId,
                response.StatusCode,
                response.Content.ReadAsStringAsync(cancellationToken).Result);

            return false;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Request cancelled for node {NodeId}", nodeValue.NodeId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending node value to API: {NodeId}", nodeValue.NodeId);
            return false;
        }
    }
}

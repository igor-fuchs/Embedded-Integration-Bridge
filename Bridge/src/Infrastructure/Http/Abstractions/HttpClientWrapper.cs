namespace Bridge.Infrastructure.Http.Abstractions;

using Bridge.Domain.Common;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

/// <summary>
/// Base HTTP client wrapper that handles common HTTP operations with consistent error handling.
/// </summary>
public class HttpClientWrapper : IHttpClientWrapper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _baseUrl;

    public HttpClientWrapper(
        HttpClient httpClient,
        ILogger logger,
        string baseUrl)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    /// <inheritdoc />
    public async Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint);

        return await ExecuteAsync(
            () => _httpClient.GetAsync(url, cancellationToken),
            async response =>
            {
                var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
                return result is null
                    ? Result<T>.Failure(ApiErrors.DeserializationError(endpoint))
                    : Result<T>.Success(result);
            },
            endpoint,
            "GET",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> PostAsync<TBody>(string endpoint, TBody body, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint);

        return await ExecuteAsync(
            () => _httpClient.PostAsJsonAsync(url, body, cancellationToken),
            endpoint,
            "POST",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> PutAsync<TBody>(string endpoint, TBody body, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint);

        return await ExecuteAsync(
            () => _httpClient.PutAsJsonAsync(url, body, cancellationToken),
            endpoint,
            "PUT",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint);

        return await ExecuteAsync(
            () => _httpClient.DeleteAsync(url, cancellationToken),
            endpoint,
            "DELETE",
            cancellationToken);
    }

    private string BuildUrl(string endpoint) =>
        $"{_baseUrl}/{endpoint.TrimStart('/')}";

    private async Task<Result> ExecuteAsync(
        Func<Task<HttpResponseMessage>> requestFunc,
        string endpoint,
        string method,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await requestFunc();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ {Method} {Endpoint} succeeded", method, endpoint);
                return Result.Success();
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "❌ {Method} {Endpoint} failed. StatusCode: {StatusCode}, Response: {Response}",
                method,
                endpoint,
                (int)response.StatusCode,
                responseContent);

            return ApiErrors.RequestFailed(endpoint, (int)response.StatusCode, responseContent);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("⏸️ {Method} {Endpoint} was cancelled", method, endpoint);
            return ApiErrors.Cancelled($"{method} {endpoint}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error executing {Method} {Endpoint}", method, endpoint);
            return ApiErrors.NetworkError(endpoint, ex.Message);
        }
    }

    private async Task<Result<T>> ExecuteAsync<T>(
        Func<Task<HttpResponseMessage>> requestFunc,
        Func<HttpResponseMessage, Task<Result<T>>> deserializeFunc,
        string endpoint,
        string method,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await requestFunc();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("✅ {Method} {Endpoint} succeeded", method, endpoint);
                return await deserializeFunc(response);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "❌ {Method} {Endpoint} failed. StatusCode: {StatusCode}, Response: {Response}",
                method,
                endpoint,
                (int)response.StatusCode,
                responseContent);

            return ApiErrors.RequestFailed(endpoint, (int)response.StatusCode, responseContent);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("⏸️ {Method} {Endpoint} was cancelled", method, endpoint);
            return ApiErrors.Cancelled($"{method} {endpoint}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error executing {Method} {Endpoint}", method, endpoint);
            return ApiErrors.NetworkError(endpoint, ex.Message);
        }
    }
}

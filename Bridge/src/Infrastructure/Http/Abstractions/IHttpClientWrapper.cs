namespace Bridge.Infrastructure.Http.Abstractions;

using Bridge.Domain.Common;

/// <summary>
/// Base interface for HTTP clients with common functionality.
/// </summary>
public interface IHttpClientWrapper
{
    /// <summary>
    /// Executes a GET request and deserializes the response.
    /// </summary>
    Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a POST request with a body.
    /// </summary>
    Task<Result> PostAsync<TBody>(string endpoint, TBody body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a PUT request with a body.
    /// </summary>
    Task<Result> PutAsync<TBody>(string endpoint, TBody body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DELETE request.
    /// </summary>
    Task<Result> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}

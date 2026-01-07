namespace Bridge.Domain.Interfaces;

using Bridge.Domain.Models;

/// <summary>
/// Interface for API client operations.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Creates a new OPC UA node value in the API.
    /// </summary>
    /// <param name="nodeValue">The node value to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request was successful, false otherwise.</returns>
    Task<bool> CreateNodeAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing OPC UA node value in the API.
    /// </summary>
    /// <param name="nodeValue">The node value to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request was successful, false otherwise.</returns>
    Task<bool> UpdateNodeAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all OPC UA nodes from the API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ApiNodesResponse with nodes if successful, null otherwise.</returns>
    Task<ApiNodesResponse?> GetAllNodesAsync(CancellationToken cancellationToken = default);
}
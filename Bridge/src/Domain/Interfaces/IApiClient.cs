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

    /// <summary>
    /// Sends a node value to the API, creating it if it doesn't exist or updating it if it does.
    /// </summary>
    /// <param name="nodeValue">The node value to send.</param>
    /// <param name="nodeExists">Whether the node is known to exist in the API.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the operation was successful, false otherwise.</returns>
    Task<bool> SendOrUpdateNodeAsync(OpcNodeValue nodeValue, bool nodeExists, CancellationToken cancellationToken = default);
}
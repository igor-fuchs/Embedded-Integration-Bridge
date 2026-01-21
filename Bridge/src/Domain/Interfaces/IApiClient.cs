namespace Bridge.Domain.Interfaces;

using Bridge.Domain.Common;
using Bridge.Domain.DTOs.Requests;
using Bridge.Domain.DTOs.Responses;

/// <summary>
/// Interface for API client operations related to OPC UA nodes.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Creates a new OPC UA node value in the API.
    /// </summary>
    /// <param name="request">The node creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<Result> CreateNodeAsync(CreateNodeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing OPC UA node value in the API.
    /// </summary>
    /// <param name="request">The node update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<Result> UpdateNodeAsync(UpdateNodeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all registered nodes from the API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing registered nodes or error details.</returns>
    Task<Result<RegisteredNodesResponse>> GetRegisteredNodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all node names from the API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing node names or error details.</returns>
    Task<Result<NodeNamesResponse>> GetNodeNamesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets command nodes from the commands-front endpoint.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing command nodes or error details.</returns>
    Task<Result<CommandNodesResponse>> GetCommandNodesAsync(CancellationToken cancellationToken = default);
}
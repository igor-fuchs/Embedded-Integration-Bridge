namespace Bridge.Domain.Interfaces;

using Bridge.Domain.Models;

/// <summary>
/// Interface for API client operations.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Sends an OPC UA node value to the API.
    /// </summary>
    /// <param name="nodeValue">The node value to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the request was successful, false otherwise.</returns>
    Task<bool> SendNodeValueAsync(OpcNodeValue nodeValue, CancellationToken cancellationToken = default);
}

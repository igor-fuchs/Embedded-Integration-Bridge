using Bridge.Domain.DTOs;

namespace Bridge.Domain.Interfaces;

/// <summary>
/// Defines the contract for OPC UA client operations.
/// </summary>
public interface IOpcUaClient : IAsyncDisposable
{
    /// <summary>
    /// Gets whether the client is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the OPC UA server asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a subscription for the specified node IDs.
    /// </summary>
    /// <param name="nodeIds">List of node IDs to monitor.</param>
    /// <param name="onValueChanged">Callback for value changes with node name and value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubscribeAsync(
        IEnumerable<string> nodeIds,
        Action<NodeDTO> onValueChanged,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the OPC UA server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
}

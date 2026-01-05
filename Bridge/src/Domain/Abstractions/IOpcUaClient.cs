namespace Bridge.Domain.Abstractions;

/// <summary>
/// Defines the contract for OPC UA client operations.
/// </summary>
public interface IOpcUaClient : IAsyncDisposable
{
    /// <summary>
    /// Connects to the OPC UA server asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a subscription for the specified node IDs.
    /// </summary>
    /// <param name="nodeIds">List of node IDs to monitor.</param>
    /// <param name="onValueChanged">Callback for value changes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SubscribeAsync(
        IEnumerable<string> nodeIds,
        Action<string, object?, DateTime> onValueChanged,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the OPC UA server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets whether the client is currently connected.
    /// </summary>
    bool IsConnected { get; }
}

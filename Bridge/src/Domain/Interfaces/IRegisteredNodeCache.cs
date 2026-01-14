namespace Bridge.Domain.Interfaces;

/// <summary>
/// Cache for tracking registered nodes in the API.
/// Determines whether nodes should be created or updated.
/// </summary>
public interface IRegisteredNodeCache
{
    /// <summary>
    /// Checks if a node is registered in the cache.
    /// </summary>
    /// <param name="nodeName">The node name to check.</param>
    /// <returns>True if the node is registered, false otherwise.</returns>
    bool IsRegistered(string nodeName);

    /// <summary>
    /// Marks a node as registered in the cache.
    /// </summary>
    /// <param name="nodeName">The node name to register.</param>
    void Register(string nodeName);

    /// <summary>
    /// Loads registered nodes from the API into the cache.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task LoadFromApiAsync(CancellationToken cancellationToken = default);
}

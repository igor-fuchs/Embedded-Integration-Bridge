namespace Bridge.Domain.Interfaces;

using Bridge.Domain.DTOs;

/// <summary>
/// Handler for processing OPC UA node value changes.
/// </summary>
public interface INodeValueChangedHandler
{
    /// <summary>
    /// Handles a node value change event.
    /// Creates new nodes or updates existing ones based on registration status.
    /// </summary>
    /// <param name="node">The node with the changed value.</param>
    Task HandleAsync(NodeDTO node);
}

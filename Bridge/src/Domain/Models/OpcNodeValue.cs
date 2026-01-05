namespace Bridge.Domain.Models;

/// <summary>
/// Represents a value change notification from an OPC UA node.
/// </summary>
/// <param name="NodeId">The identifier of the OPC UA node.</param>
/// <param name="Value">The new value of the node.</param>
/// <param name="Timestamp">The timestamp when the value changed.</param>
public sealed record OpcNodeValue(
    string NodeId,
    object? Value,
    DateTime Timestamp);

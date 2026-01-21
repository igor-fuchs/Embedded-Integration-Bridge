namespace Bridge.Domain.DTOs.Requests;

using System.Text.Json.Serialization;

/// <summary>
/// Request to create a new OPC UA node.
/// </summary>
public sealed record CreateNodeRequest
{
    /// <summary>
    /// The node identifier (NodeId).
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The node value.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}

/// <summary>
/// Request to update an existing OPC UA node.
/// </summary>
public sealed record UpdateNodeRequest
{
    /// <summary>
    /// The node identifier (NodeId). Used in the URL, not serialized to JSON.
    /// </summary>
    [JsonIgnore]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The new node value.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}
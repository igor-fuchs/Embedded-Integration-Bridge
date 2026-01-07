namespace Bridge.Domain.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a node from the API response.
/// </summary>
public sealed class ApiNode
{
    /// <summary>
    /// The node identifier (NodeId).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The node value.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }
}

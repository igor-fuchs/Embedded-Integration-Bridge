namespace Bridge.Domain.DTOs;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a node from the API response.
/// </summary>
/// <param name="Name">The node identifier (NodeId).</param>
/// <param name="Value">The node value.</param>
public sealed record NodeDTO(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("value")] object? Value
);
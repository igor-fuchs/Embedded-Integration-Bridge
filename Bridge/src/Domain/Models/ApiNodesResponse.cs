namespace Bridge.Domain.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Response model for the API nodes endpoint.
/// </summary>
public sealed class ApiNodesResponse
{
    /// <summary>
    /// List of nodes returned by the API.
    /// </summary>
    [JsonPropertyName("nodes")]
    public List<ApiNode> Nodes { get; set; } = [];

    /// <summary>
    /// Total count of nodes.
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

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

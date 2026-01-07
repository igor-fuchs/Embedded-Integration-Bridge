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

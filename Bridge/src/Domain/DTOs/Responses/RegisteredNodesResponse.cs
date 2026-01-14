namespace Bridge.Domain.DTOs.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// Response DTO for the nodes endpoint.
/// </summary>
/// <param name="Nodes">List of nodes returned by the API.</param>
/// <param name="TotalCount">Total count of nodes.</param>
public sealed record RegisteredNodesResponse(
    [property: JsonPropertyName("nodesName")] IReadOnlyList<NodeDTO> Nodes,
    [property: JsonPropertyName("totalCount")] int TotalCount
);



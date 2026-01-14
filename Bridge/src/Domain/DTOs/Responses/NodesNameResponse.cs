namespace Bridge.Domain.DTOs.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// Response DTO for the node names endpoint.
/// </summary>
/// <param name="NodeNames">List of node names returned by the API.</param>
/// <param name="TotalCount">Total count of node names.</param>
public sealed record NodeNamesResponse(
    [property: JsonPropertyName("nodeNames")] IReadOnlyList<string> NodeNames,
    [property: JsonPropertyName("totalCount")] int TotalCount
);



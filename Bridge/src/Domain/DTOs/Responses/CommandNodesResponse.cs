namespace Bridge.Domain.DTOs.Responses;

using System.Text.Json.Serialization;

/// <summary>
/// Response DTO for the commands-front endpoint.
/// Contains a list of command nodes to be written via OPC UA.
/// </summary>
/// <param name="Commands">List of command nodes from the API.</param>
public sealed record CommandNodesResponse(
    [property: JsonPropertyName("commands")] IReadOnlyList<NodeDTO> Commands
);

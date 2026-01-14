namespace Bridge.Infrastructure.Configuration;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration options for the API client.
/// </summary>
public sealed class ApiClientOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "ApiClient";

    /// <summary>
    /// The base URL of the API.
    /// </summary>
    [Url]
    [Required(ErrorMessage ="Server base URL is required.")]
    public required string BaseUrl { get; set; }

    /// <summary>
    /// The endpoint for OPC UA nodes.
    /// </summary>
    [Required(ErrorMessage ="OPC UA nodes endpoint is required.")]
    public required string OpcUaNodesEndpoint { get; set; }

    /// <summary>
    /// Timeout in seconds for HTTP requests.
    /// </summary>
    [Range(1, 300)]
    [Required(ErrorMessage ="TimeoutSeconds is required and must be between 1 and 300.")]
    public required int TimeoutSeconds { get; set; }
}

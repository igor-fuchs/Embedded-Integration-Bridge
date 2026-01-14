namespace Bridge.Infrastructure.Configuration;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration options for the OPC UA client.
/// </summary>
public sealed class OpcUaClientOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "OpcUa";

    /// <summary>
    /// The URL of the OPC UA server.
    /// </summary>
    [Required(ErrorMessage = "ServerUrl is required in appsettings.json under OpcUa section")]
    public required string ServerUrl { get; set; }

    /// <summary>
    /// Application name for the OPC UA client.
    /// </summary>
    [Required(ErrorMessage = "ApplicationName is required in appsettings.json under OpcUa section")]
    public required string ApplicationName { get; set; }

    /// <summary>
    /// Application URI for the OPC UA client.
    /// </summary>
    [Required(ErrorMessage = "ApplicationUri is required in appsettings.json under OpcUa section")]
    public required string ApplicationUri { get; set; }

    /// <summary>
    /// Product URI for the OPC UA client.
    /// </summary>
    [Required(ErrorMessage = "ProductUri is required in appsettings.json under OpcUa section")]
    public required string ProductUri { get; set; }

    /// <summary>
    /// Session timeout in milliseconds.
    /// </summary>
    [Range(1000, int.MaxValue, ErrorMessage = "SessionTimeout must be at least 1000ms")]
    public required uint SessionTimeout { get; set; }

    /// <summary>
    /// Default session timeout in milliseconds.
    /// </summary>
    [Range(1000, int.MaxValue, ErrorMessage = "DefaultSessionTimeout must be at least 1000ms")]
    public required int DefaultSessionTimeout { get; set; }

    /// <summary>
    /// Publishing interval for subscriptions in milliseconds.
    /// </summary>
    [Range(100, int.MaxValue, ErrorMessage = "PublishingInterval must be at least 100ms")]
    public required int PublishingInterval { get; set; }

    /// <summary>
    /// Retry policy options for connection attempts.
    /// </summary>
    public RetryOptions Retry { get; set; } = new();
}

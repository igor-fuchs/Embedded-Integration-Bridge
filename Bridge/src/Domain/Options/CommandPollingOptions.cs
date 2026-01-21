namespace Bridge.Domain.Options;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration options for command node polling.
/// </summary>
public sealed class CommandPollingOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "CommandPolling";

    /// <summary>
    /// Gets or sets the polling interval in milliseconds.
    /// </summary>
    [Required]
    [Range(100, 60000, ErrorMessage = "PollingIntervalMs must be between 100 and 60000")]
    public int PollingIntervalMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether command polling is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}

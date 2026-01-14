namespace Bridge.Infrastructure.Configuration;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration options for retry policies.
/// </summary>
public sealed class RetryOptions
{
    /// <summary>
    /// Maximum number of retry attempts.
    /// </summary>
    [Range(1, 20, ErrorMessage = "MaxRetries must be between 1 and 20")]
    public int MaxRetries { get; set; } = 5;

    /// <summary>
    /// Initial delay between retries in milliseconds.
    /// </summary>
    [Range(100, 60000, ErrorMessage = "InitialDelayMs must be between 100 and 60000")]
    public int InitialDelayMs { get; set; } = 1000;

    /// <summary>
    /// Maximum delay between retries in milliseconds.
    /// </summary>
    [Range(1000, 300000, ErrorMessage = "MaxDelayMs must be between 1000 and 300000")]
    public int MaxDelayMs { get; set; } = 30000;

    /// <summary>
    /// Multiplier for exponential backoff.
    /// </summary>
    [Range(1.1, 5.0, ErrorMessage = "BackoffMultiplier must be between 1.1 and 5.0")]
    public double BackoffMultiplier { get; set; } = 2.0;
}

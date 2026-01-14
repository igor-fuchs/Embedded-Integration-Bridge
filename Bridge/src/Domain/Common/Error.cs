namespace Bridge.Domain.Common;

/// <summary>
/// Represents an error with a code and description.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
public sealed record Error(string Code, string Description)
{
    /// <summary>
    /// Represents no error (empty error).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Creates an error for unexpected exceptions.
    /// </summary>
    public static Error Unexpected(string description) =>
        new("Error.Unexpected", description);

    /// <summary>
    /// Creates an error for cancelled operations.
    /// </summary>
    public static Error Cancelled(string description) =>
        new("Error.Cancelled", description);
}

/// <summary>
/// API-specific errors.
/// </summary>
public static class ApiErrors
{
    public static Error RequestFailed(string endpoint, int statusCode, string response) =>
        new("Api.RequestFailed", $"Request to '{endpoint}' failed with status {statusCode}. Response: {response}");

    public static Error NetworkError(string endpoint, string message) =>
        new("Api.NetworkError", $"Network error calling '{endpoint}': {message}");

    public static Error DeserializationError(string endpoint) =>
        new("Api.DeserializationError", $"Failed to deserialize response from '{endpoint}'");

    public static Error Cancelled(string operation) =>
        new("Api.Cancelled", $"Operation '{operation}' was cancelled");
}

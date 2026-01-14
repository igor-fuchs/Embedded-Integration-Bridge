namespace Bridge.Domain.Exceptions;

/// <summary>
/// Base exception for OPC UA related errors.
/// </summary>
public abstract class OpcUaException : Exception
{
    protected OpcUaException(string message)
        : base(message) { }

    protected OpcUaException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when connection to OPC UA server fails.
/// </summary>
public sealed class OpcUaConnectionException : OpcUaException
{
    public string ServerUrl { get; }
    public int AttemptNumber { get; }
    public int MaxRetries { get; }

    public OpcUaConnectionException(string serverUrl, int attemptNumber, int maxRetries, Exception innerException)
        : base(CreateMessage(serverUrl, attemptNumber, maxRetries), innerException)
    {
        ServerUrl = serverUrl;
        AttemptNumber = attemptNumber;
        MaxRetries = maxRetries;
    }

    private static string CreateMessage(string serverUrl, int attemptNumber, int maxRetries) =>
        $"Failed to connect to OPC UA server at '{serverUrl}' after {attemptNumber} of {maxRetries} attempts. " +
        "Please verify: 1) The server URL is correct, 2) The server is running, 3) Network connectivity is available.";
}

/// <summary>
/// Exception thrown when OPC UA endpoint discovery fails.
/// </summary>
public sealed class OpcUaEndpointNotFoundException : OpcUaException
{
    public string ServerUrl { get; }

    public OpcUaEndpointNotFoundException(string serverUrl)
        : base($"No OPC UA endpoint found at '{serverUrl}'. " +
               "Please verify the server URL and ensure the OPC UA server is properly configured.")
    {
        ServerUrl = serverUrl;
    }
}

/// <summary>
/// Exception thrown when OPC UA server discovery fails.
/// </summary>
public sealed class OpcUaServerDiscoveryException : OpcUaException
{
    public string ServerUrl { get; }

    public OpcUaServerDiscoveryException(string serverUrl, Exception innerException)
        : base($"Failed to discover OPC UA servers at '{serverUrl}'. " +
               "The server may be offline or the URL may be incorrect.", innerException)
    {
        ServerUrl = serverUrl;
    }
}

/// <summary>
/// Exception thrown when session creation fails.
/// </summary>
public sealed class OpcUaSessionException : OpcUaException
{
    public string ServerUrl { get; }

    public OpcUaSessionException(string serverUrl, Exception innerException)
        : base($"Failed to create OPC UA session with server at '{serverUrl}'. " +
               "Please check server authentication settings and client configuration.", innerException)
    {
        ServerUrl = serverUrl;
    }
}

/// <summary>
/// Exception thrown when subscription creation fails.
/// </summary>
public sealed class OpcUaSubscriptionException : OpcUaException
{
    public int NodeCount { get; }

    public OpcUaSubscriptionException(int nodeCount, Exception innerException)
        : base($"Failed to create subscription for {nodeCount} nodes. " +
               "Please verify the node IDs are valid and accessible.", innerException)
    {
        NodeCount = nodeCount;
    }
}

/// <summary>
/// Exception thrown when client is not connected.
/// </summary>
public sealed class OpcUaNotConnectedException : OpcUaException
{
    public OpcUaNotConnectedException()
        : base("OPC UA client is not connected. Call ConnectAsync before performing operations.") { }
}

/// <summary>
/// Exception thrown when all retry attempts are exhausted.
/// </summary>
public sealed class OpcUaRetryExhaustedException : OpcUaException
{
    public int TotalAttempts { get; }
    public TimeSpan TotalDuration { get; }

    public OpcUaRetryExhaustedException(string serverUrl, int totalAttempts, TimeSpan totalDuration, Exception lastException)
        : base($"All {totalAttempts} connection attempts to '{serverUrl}' failed over {totalDuration.TotalSeconds:F1} seconds. " +
               "Please check server availability and network connectivity.", lastException)
    {
        TotalAttempts = totalAttempts;
        TotalDuration = totalDuration;
    }
}

namespace Bridge.Infrastructure.Configuration;

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
    public string ServerUrl { get; set; } = "opc.tcp://192.168.1.20:4840";

    /// <summary>
    /// Application name for the OPC UA client.
    /// </summary>
    public string ApplicationName { get; set; } = "Bridge OPC UA Client";

    /// <summary>
    /// Application URI for the OPC UA client.
    /// </summary>
    public string ApplicationUri { get; set; } = "urn:BridgeClient";

    /// <summary>
    /// Product URI for the OPC UA client.
    /// </summary>
    public string ProductUri { get; set; } = "Fuchs";

    /// <summary>
    /// Session timeout in milliseconds.
    /// </summary>
    public uint SessionTimeout { get; set; } = 60000;

    /// <summary>
    /// Default session timeout in milliseconds.
    /// </summary>
    public int DefaultSessionTimeout { get; set; } = 360000;

    /// <summary>
    /// Publishing interval for subscriptions in milliseconds.
    /// </summary>
    public int PublishingInterval { get; set; } = 1000;
}

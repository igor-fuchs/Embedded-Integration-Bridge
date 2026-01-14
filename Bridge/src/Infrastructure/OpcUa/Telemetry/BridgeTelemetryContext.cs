namespace Bridge.Infrastructure.OpcUa.Telemetry;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Opc.Ua;

/// <summary>
/// Provides telemetry context for OPC UA operations including tracing, logging, and metrics.
/// </summary>
public sealed class BridgeTelemetryContext : ITelemetryContext
{
    private static readonly Lazy<BridgeTelemetryContext> _instance = new(() => new BridgeTelemetryContext());

    /// <summary>
    /// Gets the singleton instance of the telemetry context.
    /// </summary>
    public static ITelemetryContext Instance => _instance.Value;

    /// <inheritdoc/>
    public ActivitySource ActivitySource { get; }

    /// <inheritdoc/>
    public ILoggerFactory LoggerFactory { get; }

    private readonly Meter _meter;

    private BridgeTelemetryContext()
    {
        ActivitySource = new ActivitySource("Bridge.OpcUa");
        LoggerFactory = NullLoggerFactory.Instance;
        _meter = new Meter("Bridge.OpcUa");
    }

    /// <inheritdoc/>
    public Meter CreateMeter() => _meter;
}

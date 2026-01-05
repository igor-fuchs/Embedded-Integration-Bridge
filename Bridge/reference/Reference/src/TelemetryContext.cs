namespace Telemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Opc.Ua;

public sealed class TelemetryContext : ITelemetryContext
{
    public static ITelemetryContext Instance { get; } = new TelemetryContext();

    public ActivitySource ActivitySource { get; }

    public ILoggerFactory LoggerFactory { get; }

    private readonly Meter _meter;

    private TelemetryContext()
    {
        // Nome padrão usado para tracing
        ActivitySource = new ActivitySource("MeuClienteDotNet.OPCUA");

        // Logger NO-OP (não escreve em lugar nenhum)
        LoggerFactory = NullLoggerFactory.Instance;

        // Meter para métricas (pode ser plugado depois)
        _meter = new Meter("MeuClienteDotNet.OPCUA");
    }

    public Meter CreateMeter()
    {
        return _meter;
    }
}

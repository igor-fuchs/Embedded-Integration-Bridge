namespace Bridge.Application.Services;

using Bridge.Domain.Interfaces;
using Bridge.Domain.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Background service that periodically polls the API for command nodes
/// and processes them by writing values to the OPC UA server.
/// </summary>
public sealed class CommandNodePollingService : BackgroundService
{
    private readonly ICommandNodeService _commandNodeService;
    private readonly IOpcUaClient _opcUaClient;
    private readonly IRegisteredNodeCache _registeredNodeCache;
    private readonly CommandPollingOptions _options;
    private readonly ILogger<CommandNodePollingService> _logger;

    public CommandNodePollingService(
        ICommandNodeService commandNodeService,
        IOpcUaClient opcUaClient,
        IRegisteredNodeCache registeredNodeCache,
        IOptions<CommandPollingOptions> options,
        ILogger<CommandNodePollingService> logger)
    {
        _commandNodeService = commandNodeService;
        _opcUaClient = opcUaClient;
        _registeredNodeCache = registeredNodeCache;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("⏸️ Command polling is disabled");
            return;
        }

        _logger.LogInformation(
            "🚀 Command polling service started with {Interval}ms interval",
            _options.PollingIntervalMs);

        // Wait for OPC UA connection and cache initialization
        await WaitForReadinessAsync(stoppingToken);

        await RunPollingLoopAsync(stoppingToken);
    }

    /// <summary>
    /// Waits until the OPC UA client is connected and the node cache is loaded.
    /// </summary>
    private async Task WaitForReadinessAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("⏳ Waiting for OPC UA connection and cache initialization...");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_opcUaClient.IsConnected)
            {
                _logger.LogInformation("✅ OPC UA client is connected, command polling is ready");
                break;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    /// <summary>
    /// Runs the main polling loop that fetches and processes command nodes.
    /// </summary>
    private async Task RunPollingLoopAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_options.PollingIntervalMs));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (!_opcUaClient.IsConnected)
                {
                    _logger.LogWarning("⚠️ OPC UA client disconnected, skipping command poll");
                    continue;
                }

                await _commandNodeService.ProcessCommandsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("🛑 Command polling cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during command polling cycle");
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Command polling service stopping...");
        await base.StopAsync(cancellationToken);
    }
}

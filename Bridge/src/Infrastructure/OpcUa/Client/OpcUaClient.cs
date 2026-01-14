namespace Bridge.Infrastructure.OpcUa.Client;

using Bridge.Domain.DTOs;
using Bridge.Domain.Exceptions;
using Bridge.Domain.Interfaces;
using Bridge.Infrastructure.Configuration;
using Bridge.Infrastructure.OpcUa.Telemetry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Client;
using System.Diagnostics;

/// <summary>
/// OPC UA client implementation for connecting to and monitoring OPC UA servers.
/// Includes retry logic with exponential backoff for connection resilience.
/// </summary>
public sealed class OpcUaClient : IOpcUaClient
{
    private readonly OpcUaClientOptions _options;
    private readonly ILogger<OpcUaClient> _logger;

    private ISession? _session;
    private Subscription? _subscription;
    private bool _disposed;

    /// <inheritdoc/>
    public bool IsConnected => _session?.Connected ?? false;

    #region Constructor

    public OpcUaClient(
        IOptions<OpcUaClientOptions> options,
        ILogger<OpcUaClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    #endregion

    #region Public Methods
    
    /// <inheritdoc/>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsConnected)
        {
            _logger.LogWarning("Already connected to OPC UA server");
            return;
        }

        var serverUri = new Uri(_options.ServerUrl);
        _session = await ConnectWithRetryAsync(serverUri, cancellationToken);

        _logger.LogInformation("✅ Successfully connected to OPC UA server at {ServerUrl}", _options.ServerUrl);
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(
        IEnumerable<string> nodeIds,
        Action<NodeDTO> onValueChanged,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_session is null || !_session.Connected)
        {
            throw new OpcUaNotConnectedException();
        }

        var nodeIdList = nodeIds.ToList();

        try
        {
            _subscription = await CreateSubscriptionAsync(nodeIdList, onValueChanged, cancellationToken);
            _logger.LogDebug("Subscription created with {Count} monitored items", _subscription.MonitoredItemCount);
        }
        catch (Exception ex) when (ex is not OpcUaException)
        {
            throw new OpcUaSubscriptionException(nodeIdList.Count, ex);
        }
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_subscription is not null)
        {
            await _subscription.DeleteAsync(silent: true);
            _subscription = null;
        }

        if (_session is not null)
        {
            await _session.CloseAsync(cancellationToken);
            _session.Dispose();
            _session = null;
        }

        _logger.LogInformation("Disconnected from OPC UA server");
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await DisconnectAsync();
        _disposed = true;
    }

    #endregion

    #region Private Methods - Connection with Retry

    private async Task<ISession> ConnectWithRetryAsync(Uri serverUri, CancellationToken cancellationToken)
    {
        var retryOptions = _options.Retry;
        var stopwatch = Stopwatch.StartNew();
        Exception? lastException = null;
        var currentDelay = retryOptions.InitialDelayMs;

        for (var attempt = 1; attempt <= retryOptions.MaxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation(
                    "🔄 Attempting to connect to OPC UA server (attempt {Attempt}/{MaxRetries})...",
                    attempt,
                    retryOptions.MaxRetries);

                var session = await CreateSessionAsync(serverUri, cancellationToken);
                
                stopwatch.Stop();
                _logger.LogInformation(
                    "✅ Connected successfully on attempt {Attempt} after {Duration:F1}s",
                    attempt,
                    stopwatch.Elapsed.TotalSeconds);

                return session;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("⏸️ Connection attempt cancelled");
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;
                LogConnectionError(ex, attempt, retryOptions.MaxRetries);

                if (attempt == retryOptions.MaxRetries)
                {
                    break;
                }

                await WaitBeforeRetryAsync(attempt, currentDelay, retryOptions.MaxRetries, cancellationToken);
                currentDelay = CalculateNextDelay(currentDelay, retryOptions);
            }
        }

        stopwatch.Stop();
        throw new OpcUaRetryExhaustedException(
            _options.ServerUrl,
            retryOptions.MaxRetries,
            stopwatch.Elapsed,
            lastException!);
    }

    private void LogConnectionError(Exception ex, int attempt, int maxRetries)
    {
        var remainingAttempts = maxRetries - attempt;

        if (remainingAttempts > 0)
        {
            _logger.LogWarning(
                "❌ Connection attempt {Attempt} failed: {Message}. {Remaining} attempts remaining.",
                attempt,
                ex.Message,
                remainingAttempts);
        }
        else
        {
            _logger.LogError(
                ex,
                "❌ Connection attempt {Attempt} failed: {Message}. No more attempts remaining.",
                attempt,
                ex.Message);
        }
    }

    private async Task WaitBeforeRetryAsync(int attempt, int delayMs, int maxRetries, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "⏳ Waiting {Delay:F1}s before retry {NextAttempt}/{MaxRetries}...",
            delayMs / 1000.0,
            attempt + 1,
            maxRetries);

        await Task.Delay(delayMs, cancellationToken);
    }

    private static int CalculateNextDelay(int currentDelay, RetryOptions options)
    {
        var nextDelay = (int)(currentDelay * options.BackoffMultiplier);
        return Math.Min(nextDelay, options.MaxDelayMs);
    }

    #endregion

    #region Private Methods - Session Creation

    private async Task<ISession> CreateSessionAsync(Uri serverUri, CancellationToken cancellationToken)
    {
        var config = CreateApplicationConfiguration();
        var selectedEndpoint = await DiscoverEndpointAsync(config, serverUri);

        if (selectedEndpoint is null)
        {
            throw new OpcUaEndpointNotFoundException(serverUri.ToString());
        }

        try
        {
            return await CreateSessionFromEndpointAsync(config, selectedEndpoint, cancellationToken);
        }
        catch (Exception ex) when (ex is not OpcUaException)
        {
            throw new OpcUaSessionException(serverUri.ToString(), ex);
        }
    }

    private ApplicationConfiguration CreateApplicationConfiguration() =>
        new()
        {
            ApplicationName = _options.ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = _options.ApplicationUri,
            ProductUri = _options.ProductUri,
            ClientConfiguration = new ClientConfiguration
            {
                DefaultSessionTimeout = _options.DefaultSessionTimeout
            }
        };

    private async Task<ISession> CreateSessionFromEndpointAsync(
        ApplicationConfiguration config,
        EndpointDescription endpoint,
        CancellationToken cancellationToken)
    {
        var endpointConfiguration = EndpointConfiguration.Create(config);
        var configuredEndpoint = new ConfiguredEndpoint(null, endpoint, endpointConfiguration);
        var userIdentity = new UserIdentity(new AnonymousIdentityToken());

        ISessionFactory sessionFactory = new DefaultSessionFactory(BridgeTelemetryContext.Instance);

        return await sessionFactory.CreateAsync(
            configuration: config,
            endpoint: configuredEndpoint,
            updateBeforeConnect: true,
            sessionName: $"{_options.ApplicationName} Session",
            sessionTimeout: _options.SessionTimeout,
            identity: userIdentity,
            preferredLocales: null,
            ct: cancellationToken);
    }

    private async Task<EndpointDescription?> DiscoverEndpointAsync(
        ApplicationConfiguration config,
        Uri serverUri)
    {
        try
        {
            using var discoveryClient = await DiscoveryClient.CreateAsync(config, serverUri);
            var servers = await discoveryClient.FindServersAsync(null);

            if (servers.Count == 0)
            {
                return null;
            }

            var firstServer = servers[0];
            if (firstServer.DiscoveryUrls.Count == 0)
            {
                return null;
            }

            var discoveryUrl = new Uri(firstServer.DiscoveryUrls[0]);

            using var endpointDiscovery = await DiscoveryClient.CreateAsync(config, discoveryUrl);
            var endpoints = await endpointDiscovery.GetEndpointsAsync(null);

            return endpoints.Count > 0 ? endpoints[0] : null;
        }
        catch (Exception ex) when (ex is not OpcUaException)
        {
            throw new OpcUaServerDiscoveryException(serverUri.ToString(), ex);
        }
    }

    #endregion

    #region Private Methods - Subscription

    private async Task<Subscription> CreateSubscriptionAsync(
        IEnumerable<string> nodeIds,
        Action<NodeDTO> onValueChanged,
        CancellationToken cancellationToken)
    {
        var subscription = new Subscription(_session!.DefaultSubscription)
        {
            PublishingInterval = _options.PublishingInterval
        };

        foreach (var nodeId in nodeIds)
        {
            var monitoredItem = new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = nodeId,
                StartNodeId = new NodeId(nodeId)
            };

            monitoredItem.Notification += (item, e) =>
            {
                foreach (var value in item.DequeueValues())
                {
                    onValueChanged(new NodeDTO(item.DisplayName, value.Value));
                }
            };

            subscription.AddItem(monitoredItem);
        }

        _session!.AddSubscription(subscription);
        await subscription.CreateAsync(cancellationToken);

        return subscription;
    }

    #endregion
}

namespace Bridge.Infrastructure.OpcUa;

using Bridge.Domain.Abstractions;
using Bridge.Infrastructure.Configuration;
using Bridge.Infrastructure.Telemetry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Opc.Ua.Client;

/// <summary>
/// OPC UA client implementation for connecting to and monitoring OPC UA servers.
/// </summary>
public sealed class OpcUaClient : IOpcUaClient
{
    private readonly OpcUaClientOptions _options;
    private readonly ILogger<OpcUaClient> _logger;

    private ISession? _session;
    private Subscription? _subscription;
    private bool _disposed;

    public OpcUaClient(
        IOptions<OpcUaClientOptions> options,
        ILogger<OpcUaClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsConnected => _session?.Connected ?? false;

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
        _session = await CreateSessionAsync(serverUri, cancellationToken);

        _logger.LogInformation("Successfully connected to OPC UA server at {ServerUrl}", _options.ServerUrl);
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(
        IEnumerable<string> nodeIds,
        Action<string, object?, DateTime> onValueChanged,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_session is null || !_session.Connected)
        {
            throw new InvalidOperationException("Not connected to OPC UA server. Call ConnectAsync first.");
        }

        _subscription = await CreateSubscriptionAsync(nodeIds, onValueChanged, cancellationToken);

        _logger.LogDebug("Subscription created with {Count} monitored items", _subscription.MonitoredItemCount);
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

    #region Private Methods

    private async Task<ISession> CreateSessionAsync(Uri serverUri, CancellationToken cancellationToken)
    {
        var config = new ApplicationConfiguration
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

        var selectedEndpoint = await DiscoverEndpointAsync(config, serverUri);

        if (selectedEndpoint is null)
        {
            throw new InvalidOperationException($"No OPC UA endpoint found at {serverUri}");
        }

        var endpointConfiguration = EndpointConfiguration.Create(config);
        var configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        var userIdentity = new UserIdentity(new AnonymousIdentityToken());

        ISessionFactory sessionFactory = new DefaultSessionFactory(BridgeTelemetryContext.Instance);

        var session = await sessionFactory.CreateAsync(
            configuration: config,
            endpoint: configuredEndpoint,
            updateBeforeConnect: true,
            sessionName: $"{_options.ApplicationName} Session",
            sessionTimeout: _options.SessionTimeout,
            identity: userIdentity,
            preferredLocales: null,
            ct: cancellationToken);

        return session;
    }

    private static async Task<EndpointDescription?> DiscoverEndpointAsync(
        ApplicationConfiguration config,
        Uri serverUri)
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

    private async Task<Subscription> CreateSubscriptionAsync(
        IEnumerable<string> nodeIds,
        Action<string, object?, DateTime> onValueChanged,
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
                    onValueChanged(item.DisplayName, value.Value, DateTime.UtcNow);
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

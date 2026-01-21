namespace Services;

using Opc.Ua;
using Opc.Ua.Client;
using Telemetry;

/// <summary>
/// Encapsula a lógica de conexão OPC UA, criação de subscription e broadcast via WebSocketServer.
/// </summary>
public class OpcUaClient
{
    /// <summary>
    /// Sessão OPC UA ativa.
    /// </summary>
    private ISession session;
    /// <summary>
    /// Subscription OPC UA ativa.
    /// </summary>
    private Subscription subscription;

    /// <summary>
    /// Construtor que cria a sessão OPC UA e a subscription para os NodeIds fornecidos.
    /// </summary>
    /// <param name="serverUrl">URL do servidor OPC UA.</param>
    /// <param name="listNodeIds">Lista de NodeIds para a subscription.</param>
    /// <param name="OnSubscriptionEvent">Callback para eventos da subscription.</param>
    public OpcUaClient(Uri serverUrl, List<string> listNodeIds)
    {
        session = createAsyncSession(serverUrl).GetAwaiter().GetResult();
        subscription = createAsyncSubscription(listNodeIds).GetAwaiter().GetResult();
    }

    #region Private Methods

    /// <summary>
    /// Cria uma sessão OPC UA assíncrona.
    /// </summary>
    /// <param name="serverUri">URI do servidor OPC UA.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a sessão criada.</returns>
    private async Task<ISession> createAsyncSession(Uri serverUri)
    {   
        var config = new ApplicationConfiguration
        {
            ApplicationName = "UA Client 1500",
            ApplicationType = ApplicationType.Client,
            ApplicationUri = "urn:MyClient",
            ProductUri = "Fuchs",
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 360000 }
        };

        EndpointDescription selectedEndpoint = new EndpointDescription();
        using (DiscoveryClient discoveryClient = await DiscoveryClient.CreateAsync(config, serverUri))
        {
            var servers = await discoveryClient.FindServersAsync(null);
            if (servers.Count > 0)
            {
                var firstServer = servers[0];
                var firstDiscoveryUrl = new Uri(firstServer.DiscoveryUrls[0]);

                using (var endpointDiscovery = await DiscoveryClient.CreateAsync(config, firstDiscoveryUrl))
                {
                    var endpoints = await endpointDiscovery.GetEndpointsAsync(null);
                    if (endpoints.Count > 0)
                    {
                        selectedEndpoint = endpoints[0];
                    }
                }
            }
        }

        if (selectedEndpoint == null)
        {
            throw new InvalidOperationException("Nenhum endpoint OPC UA encontrado.");
        }

        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(config);
        ConfiguredEndpoint configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
        UserIdentity userIdentity = new UserIdentity(new AnonymousIdentityToken());

        ISessionFactory sessionFactory = new DefaultSessionFactory(TelemetryContext.Instance);
        ISession session = await sessionFactory.CreateAsync(
            configuration: config,
            endpoint: configuredEndpoint,
            updateBeforeConnect: true,
            sessionName: "MySession",
            sessionTimeout: 60000,
            identity: userIdentity,
            preferredLocales: null,
            ct: default
        );

        return session;
    }

    /// <summary>
    /// Cria uma subscription OPC UA assíncrona para os NodeIds fornecidos.
    /// </summary>
    /// <param name="listNodeIds">Lista de NodeIds para monitorar.</param
    /// <param name="OnSubscriptionEvent">Callback para eventos da subscription.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a subscription criada.</returns>
    private async Task<Subscription> createAsyncSubscription(List<string> listNodeIds)
    {
        // Create subscription
        subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };

        foreach (var nodeId in listNodeIds)
        {
            var display = nodeId;
            var monitoredItem = new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = display,
                StartNodeId = new NodeId(display)
            };

            // Handler para notificações de mudança de valor
            monitoredItem.Notification += (item, e) =>
            {
                foreach (var value in item.DequeueValues())
                {
                    var payload = new
                    {
                        node = item.DisplayName,
                        timestamp = DateTime.UtcNow,
                        value = value.Value
                    };

                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {item.DisplayName} => {value.Value}");
                }
            };

            subscription.AddItem(monitoredItem);
        }

        session.AddSubscription(subscription);
        await subscription.CreateAsync();

        return subscription;
    }

    #endregion Private Methods

}


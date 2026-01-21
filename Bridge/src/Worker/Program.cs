using Bridge.Application;
using Bridge.Application.Services;
using Bridge.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

// Add infrastructure services
builder.Services.AddInfrastructure();

// Add application services
builder.Services.AddApplication();

// Add hosted services
builder.Services.AddHostedService<OpcUaMonitoringService>();

var host = builder.Build();

Console.WriteLine("🚀 Bridge OPC UA Client starting...");
Console.WriteLine("📡 Monitoring NodeIds for changes. Press Ctrl+C to stop.");

await host.RunAsync();

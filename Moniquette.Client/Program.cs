using Microsoft.Extensions.DependencyInjection;
using Moniquette.Client;
using Moniquette.Client.Extensions;

 var serviceProvider = new ServiceCollection()
        .AddLoggingReady()
        .AddConfiguration()
        .AddGrpcChannel()
        .AddHttpClient()
        .AddHardwareInfo()
        .AddServices()
        .AddFillers()
        .AddApi()
        .BuildServiceProvider();
var agent = new Agent(serviceProvider);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
        Console.WriteLine("\r\rCtrl+C detected, stopping...");
        cts.Cancel(true);
        e.Cancel = true; // don't kill the process before exiting
};
await agent.ExecuteAsync(cts.Token);


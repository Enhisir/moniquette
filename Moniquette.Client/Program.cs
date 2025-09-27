using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moniquette.Client;
using Moniquette.Client.Extensions;

var serviceProvider = new ServiceCollection()
        .AddLoggingReady()
        .AddConfiguration()
        .AddHttpClient()
        .AddHardwareInfo()
        .AddFillers()
        .AddApi()
        .BuildServiceProvider();
var agent = new Agent(serviceProvider);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
        Console.WriteLine("Ctrl+C detected, stopping...");
        cts.Cancel();
        e.Cancel = true; // don't kill the process before exiting
};
await agent.ExecuteAsync(cts.Token);

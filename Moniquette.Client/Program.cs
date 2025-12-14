using Microsoft.Extensions.DependencyInjection;
using Moniquette.Client;
using Moniquette.Client.Extensions;

 var serviceProvider = new ServiceCollection()
        .AddLoggingReady()
        .AddConfiguration()
        .AddGrpcChannel()
        .AddServices()
        .AddHttpClient()
        .AddHardwareInfo()
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


/*
using System.Text.Json;
using Moniquette.Client.Pipeline.Helpers;
using Tmds.DBus.Protocol;
using Connection = Tmds.DBus.Protocol.Connection;

var connection = new Connection(Address.Session!);
await connection.ConnectAsync();

var result = await LinuxViewHelper.ListActiveViews();
Console.WriteLine(result);

public class ActiveView
{
    public string Class { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int Pid { get; set; }
    public long Id { get; set; }
    public int Maximized { get; set; }
    public bool Focus { get; set; }
}
*/

/*
var service = new WmctrlService();
var views = WmctrlService.GetX11Views();
foreach (var view in views)
{
    Console.WriteLine(view);
}
*/


using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Moniquette.Common.Extensions;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Pipeline.Fillers;

public class NetworkFiller(ILogger<NetworkFiller> logger) : IReportFiller
{
    public Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        var interfaces = NetworkInterface
            .GetAllNetworkInterfaces();
        interfaces = interfaces
            .Where(i => i.OperationalStatus == OperationalStatus.Up)
            .Where(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .ToArray();

        report.Connections = interfaces
            .Select<NetworkInterface, NetworkConnection>(network =>
            {
                var dns = BaseUtils.TryInvoke(() => network
                    .GetIPProperties()
                    .DnsAddresses
                    .Select(ip => ip.ToString())
                    .ToList(), []);
                var gw = BaseUtils.TryInvoke(() => network
                    .GetIPProperties()
                    .GatewayAddresses
                    .Select(g => g.Address.ToString())
                    .ToList(), []);

                var connection = new NetworkConnection
                {
                    Name = network.Name.ToLower(),
                    Description = network.Description.ToLower(),
                    InterfaceType = network.NetworkInterfaceType,
                    DomainNameServices = dns!,
                    Gateways = gw!
                };
                try
                {
                    var ip = network.GetIPProperties().GetDefaultIpAddress().ToString();
                    var mac = NetworkUtils.GetBeautifulMacAddressString(network.GetPhysicalAddress());
                    connection.IpAddressString = ip.Equals(string.Empty) ? null : ip;
                    connection.MacAddressString = mac.Equals(string.Empty) ? null : mac;
                }
                catch (Exception exception)
                {
                    logger.LogWarning(exception, "Error while gathering info about {connectionName}", connection.Name);
                }

                return connection;
            })
            .ToList();

        return Task.FromResult(report);
    }
}
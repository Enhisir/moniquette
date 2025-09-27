using System.Net.NetworkInformation;
using Moniquette.Common;
using Moniquette.Common.Extensions;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Pipeline.Fillers;

public class NetworkReportFiller : IReportFiller
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
            .Select<NetworkInterface, NetworkConnection>(network => {
                var dns = BaseUtils.SafeGet(() => network
                    .GetIPProperties()
                    .DnsAddresses
                    .Select(ip => ip.ToString())
                    .ToList(), []);
                var gw = BaseUtils.SafeGet(() => network
                    .GetIPProperties()
                    .GatewayAddresses
                    .Select(g => g.Address.ToString())
                    .ToList(), []);
                
                var connection = new NetworkConnection()
                {
                    Name = network.Name.ToLower(),
                    Description = network.Description.ToLower(),
                    InterfaceType = network.NetworkInterfaceType,
                    IpAddressString = network.GetIPProperties().GetDefaultIpAddress().ToString(),
                    MacAddressString = NetworkUtils.GetBeautifulMacAddressString(network.GetPhysicalAddress()),
                    DomainNameServices = dns!,
                    Gateways = gw! 
                };
                return connection;
            })
            .ToList();
        
        return Task.FromResult(report);
    }
}
using System.Net.NetworkInformation;

namespace Moniquette.Server.Analysis;

public class NetworkPolicyOptions
{
    public int RequiredConnectionCount { get; set; } = 1;

    public List<AllowedNetworkConnectionOptions> AllowedConnections { get; set; } = [];
}

public class AllowedNetworkConnectionOptions
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public NetworkInterfaceType? InterfaceType { get; set; }

    public string? IpAddress { get; set; }

    public string? MacAddress { get; set; }

    public List<string> Gateways { get; set; } = [];

    public List<string> DnsServers { get; set; } = [];
}

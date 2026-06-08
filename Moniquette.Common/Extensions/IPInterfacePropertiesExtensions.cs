using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Moniquette.Common.Extensions;

public static class IpInterfacePropertiesExtensions
{
    public static IPAddress GetDefaultIpAddress(this IPInterfaceProperties props) 
        => props
            .UnicastAddresses
            .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(info => info.Address)
            .FirstOrDefault() 
           ?? throw new Exception("Ip address not found. IP interface info: ???");
}

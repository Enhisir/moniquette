using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Moniquette.Common.Models;

public class NetworkConnection
{
    [Required] public string Name { get; set; } = null!;
    [Required] public string Description { get; set; } = null!;
    [Required] public NetworkInterfaceType InterfaceType { get; set; }

    [Required] public string? IpAddressString { get; set; }
    [Required] public string? MacAddressString { get; set; }

    /// List of DNS IPs
    [Required]
    public List<string> DomainNameServices { get; set; } = null!;

    /// List of Gateways (points to route packages)
    [Required]
    public List<string> Gateways { get; set; } = null!;

    [JsonIgnore, IgnoreDataMember]
    public IPAddress? Address
        => IpAddressString is null ? null : IPAddress.Parse(IpAddressString);

    [JsonIgnore, IgnoreDataMember]
    public PhysicalAddress MacAddress
        => PhysicalAddress.Parse(MacAddressString);
}
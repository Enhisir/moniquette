using System.Net.NetworkInformation;

namespace Moniquette.Common.Utils;

public static class NetworkUtils
{
    public static string GetBeautifulMacAddressString(PhysicalAddress macAddress)
        => string.Join(":", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));
}
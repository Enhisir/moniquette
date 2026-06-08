using Microsoft.Extensions.Options;
using Moniquette.Common.Enums;
using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public class NetworkAnalyzer(IOptions<NetworkPolicyOptions> options) : AnalyzerBase
{
    private readonly NetworkPolicyOptions policy = options.Value;

    public override Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var threats = new List<Threat>();
        var connections = report.Connections;

        if (connections.Count != policy.RequiredConnectionCount)
        {
            threats.Add(CreateThreat(
                report,
                ThreatType.Warning,
                $"Нарушение сетевой политики: ожидалось активных подключений: {policy.RequiredConnectionCount}, фактически: {connections.Count}."));
        }

        if (policy.AllowedConnections.Count == 0)
        {
            return Task.FromResult<IReadOnlyCollection<Threat>>(threats);
        }

        foreach (var connection in connections)
        {
            if (policy.AllowedConnections.Any(allowed => Matches(connection, allowed)))
            {
                continue;
            }

            threats.Add(CreateThreat(
                report,
                ThreatType.Warning,
                $"Сетевое подключение не совпадает с разрешенными профилями: {FormatConnection(connection)}."));
        }

        return Task.FromResult<IReadOnlyCollection<Threat>>(threats);
    }

    private static bool Matches(
        NetworkConnection connection,
        AllowedNetworkConnectionOptions allowed)
    {
        return MatchesString(connection.Name, allowed.Name)
               && MatchesString(connection.Description, allowed.Description)
               && MatchesInterfaceType(connection, allowed)
               && MatchesString(connection.IpAddressString, allowed.IpAddress)
               && MatchesMacAddress(connection.MacAddressString, allowed.MacAddress)
               && MatchesSet(connection.Gateways, allowed.Gateways)
               && MatchesSet(connection.DomainNameServices, allowed.DnsServers);
    }

    private static bool MatchesString(string? actual, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return true;
        }

        return string.Equals(
            Normalize(actual),
            Normalize(expected),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesMacAddress(string? actual, string? expected)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return true;
        }

        return string.Equals(
            NormalizeMacAddress(actual),
            NormalizeMacAddress(expected),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesInterfaceType(
        NetworkConnection connection,
        AllowedNetworkConnectionOptions allowed)
        => allowed.InterfaceType is null || connection.InterfaceType == allowed.InterfaceType;

    private static bool MatchesSet(
        IReadOnlyCollection<string> actual,
        IReadOnlyCollection<string> expected)
    {
        if (expected.Count == 0)
        {
            return true;
        }

        return actual
            .Select(Normalize)
            .Order(StringComparer.OrdinalIgnoreCase)
            .SequenceEqual(
                expected.Select(Normalize).Order(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
    }

    private static string Normalize(string? value)
        => value?.Trim() ?? string.Empty;

    private static string NormalizeMacAddress(string? value)
        => new((value ?? string.Empty)
            .Where(char.IsAsciiHexDigit)
            .Select(char.ToUpperInvariant)
            .ToArray());

    private static string FormatConnection(NetworkConnection connection)
        => $"{connection.Name}, type={connection.InterfaceType}, ip={connection.IpAddressString ?? "none"}, mac={connection.MacAddressString ?? "none"}, gateways=[{string.Join(", ", connection.Gateways)}], dns=[{string.Join(", ", connection.DomainNameServices)}]";
}

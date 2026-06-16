using Moniquette.Common.Enums;
using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public class VirtualizationAnalyzer : AnalyzerBase
{
    private const int MaxHits = 25;

    private static readonly string[] VirtualizationMarkers =
    [
        "virtualbox",
        "vmware",
        "qemu",
        "kvm",
        "hyper-v",
        "hyperv",
        "parallels",
        "xen",
        "bochs",
        "virtual machine",
        "virtual platform"
    ];

    private static readonly string[] RegistryVirtualizationMarkers =
    [
        "vmware tools",
        "vmware svga",
        "vmhgfs",
        "vmmouse",
        "vmxnet",
        "vmci",
        "virtualbox guest additions",
        "oracle vm virtualbox",
        "vboxguest",
        "vboxservice",
        "vboxsf",
        "vboxvideo",
        "hyper-v",
        "hyperv",
        "vmbus",
        "vmguest",
        "vmicheartbeat",
        "vmicshutdown",
        "vmicvss",
        "qemu-ga",
        "qemu guest agent",
        "virtio",
        "red hat virtio",
        "xenservice",
        "xenbus",
        "parallels tools",
        "prl_tools"
    ];

    private static readonly (string Prefix, string Vendor)[] VirtualMacPrefixes =
    [
        ("000569", "VMware"),
        ("000C29", "VMware"),
        ("001C14", "VMware"),
        ("005056", "VMware"),
        ("080027", "VirtualBox"),
        ("00155D", "Hyper-V"),
        ("525400", "QEMU/KVM"),
        ("00163E", "Xen"),
        ("001C42", "Parallels")
    ];

    public override Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var hardware = report.HardwareInfo;
        var hits = new List<string>();

        AddHit(hits, "ОС", hardware?.OperatingSystem, VirtualizationMarkers);
        AddHit(hits, "BIOS manufacturer", hardware?.Bios?.Manufacturer, VirtualizationMarkers);
        AddHit(hits, "BIOS version", hardware?.Bios?.Version, VirtualizationMarkers);
        AddHit(hits, "System vendor", hardware?.ComputerSystem?.Vendor, VirtualizationMarkers);
        AddHit(hits, "System name", hardware?.ComputerSystem?.Name, VirtualizationMarkers);
        AddHit(hits, "Motherboard manufacturer", hardware?.Motherboard?.Manufacturer, VirtualizationMarkers);
        AddHit(hits, "Motherboard product", hardware?.Motherboard?.Product, VirtualizationMarkers);
        AddHit(hits, "CPU", hardware?.Cpu?.Name, VirtualizationMarkers);
        AddRegistryHits(hits, report.WindowsRegistry);
        AddNetworkHits(hits, report.Connections);

        IReadOnlyCollection<Threat> threats = hits.Count == 0
            ? []
            :
            [
                CreateThreat(
                    report,
                    ThreatType.Warning,
                    $"Обнаружены технические признаки виртуализации: {string.Join("; ", hits.Take(MaxHits))}.")
            ];

        return Task.FromResult(threats);
    }

    private static void AddRegistryHits(
        List<string> hits,
        IReadOnlyDictionary<string, string>? registry)
    {
        if (registry is null)
        {
            return;
        }

        foreach (var (key, value) in registry)
        {
            if (hits.Count >= MaxHits)
            {
                return;
            }

            var entry = $"{key}={value}";
            if (ContainsAny(entry, RegistryVirtualizationMarkers))
            {
                AddUniqueHit(hits, $"Windows Registry: {entry}");
            }
        }
    }

    private static void AddNetworkHits(
        List<string> hits,
        IReadOnlyCollection<NetworkConnection>? connections)
    {
        if (connections is null)
        {
            return;
        }

        foreach (var connection in connections)
        {
            if (hits.Count >= MaxHits)
            {
                return;
            }

            AddHit(hits, "Сетевой адаптер", connection.Name, VirtualizationMarkers);
            AddHit(hits, "Описание сетевого адаптера", connection.Description, VirtualizationMarkers);
            AddVirtualMacHit(hits, connection);
        }
    }

    private static void AddVirtualMacHit(List<string> hits, NetworkConnection connection)
    {
        var normalizedMac = NormalizeMacAddress(connection.MacAddressString);
        if (normalizedMac.Length < 6)
        {
            return;
        }

        foreach (var (prefix, vendor) in VirtualMacPrefixes)
        {
            if (normalizedMac.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                AddUniqueHit(
                    hits,
                    $"MAC сетевого адаптера {connection.Name}: {connection.MacAddressString} ({vendor})");
                return;
            }
        }
    }

    private static void AddHit(
        List<string> hits,
        string fieldName,
        string? value,
        params string[] markers)
    {
        if (ContainsAny(value, markers))
        {
            AddUniqueHit(hits, $"{fieldName}: {value}");
        }
    }

    private static void AddUniqueHit(List<string> hits, string hit)
    {
        if (hits.Count >= MaxHits || hits.Contains(hit, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        hits.Add(hit);
    }

    private static string NormalizeMacAddress(string? value)
        => new((value ?? string.Empty)
            .Where(char.IsAsciiHexDigit)
            .Select(char.ToUpperInvariant)
            .ToArray());
}

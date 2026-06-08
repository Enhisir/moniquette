using Moniquette.Common.Enums;
using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public class VirtualizationAnalyzer : AnalyzerBase
{
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

    public override Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var hardware = report.HardwareInfo;
        var hits = new List<string>();

        AddHit(hits, "ОС", hardware.OperatingSystem);
        AddHit(hits, "BIOS manufacturer", hardware.Bios?.Manufacturer);
        AddHit(hits, "BIOS version", hardware.Bios?.Version);
        AddHit(hits, "System vendor", hardware.ComputerSystem?.Vendor);
        AddHit(hits, "System name", hardware.ComputerSystem?.Name);
        AddHit(hits, "Motherboard manufacturer", hardware.Motherboard?.Manufacturer);
        AddHit(hits, "Motherboard product", hardware.Motherboard?.Product);
        AddHit(hits, "CPU", hardware.Cpu?.Name);

        IReadOnlyCollection<Threat> threats = hits.Count == 0
            ? []
            :
            [
                CreateThreat(
                    report,
                    ThreatType.Warning,
                    $"Обнаружены признаки виртуализации: {string.Join("; ", hits)}.")
            ];

        return Task.FromResult(threats);
    }

    private static void AddHit(List<string> hits, string fieldName, string? value)
    {
        if (ContainsAny(value, VirtualizationMarkers))
        {
            hits.Add($"{fieldName}: {value}");
        }
    }
}

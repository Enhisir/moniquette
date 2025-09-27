using System.Text.Json.Serialization;

namespace Moniquette.Common.Models;

public class Report
{
    public List<string> Views { get; set; } = null!;
    public HardwareBriefInfo HardwareInfo { get; set; } = null!;
    public List<NetworkConnection> Connections { get; set; } = null!;
};
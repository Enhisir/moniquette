using Moniquette.Common.Models;

namespace Moniquette.Common.Dto;

public class ReportDto
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public DateTime Timestamp { get; set; }

    public List<ProcessInfo> Processes { get; set; } = [];

    public HardwareBriefInfo HardwareInfo { get; set; } = null!;

    public List<NetworkConnection> Connections { get; set; } = [];

    public Dictionary<string, string> WindowsRegistry { get; set; } = [];

    public bool IsDockerEnabled { get; set; }

    public List<RunningDockerContainer> DockerContainers { get; set; } = [];
}

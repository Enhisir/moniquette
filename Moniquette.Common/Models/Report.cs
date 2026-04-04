

namespace Moniquette.Common.Models;

public class Report
{
    public Guid SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public List<ProcessInfo> Processes { get; set; } = null!;
    public HardwareBriefInfo HardwareInfo { get; set; } = null!;
    public List<NetworkConnection> Connections { get; set; } = null!;
    public Dictionary<string, string>? WindowsRegistry { get; set; }
    public bool IsDockerEnabled { get; set; }
    public List<RunningDockerContainer>? DockerContainers { get; set; }
}
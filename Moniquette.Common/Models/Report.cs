

namespace Moniquette.Common.Models;

public class Report
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid SessionId { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public List<ProcessInfo> Processes { get; set; } = [];
    
    public HardwareBriefInfo HardwareInfo { get; set; } = null!;
    
    public List<NetworkConnection> Connections { get; set; } = [];
    
    public Dictionary<string, string> WindowsRegistry { get; set; } = [];
    
    public bool IsDockerEnabled { get; set; }
    
    public List<RunningDockerContainer> DockerContainers { get; set; } = [];
}

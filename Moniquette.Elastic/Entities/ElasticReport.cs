using Moniquette.Common.Models;

namespace Moniquette.Elastic.Entities;

public class ElasticReport
{
    public Guid SessionId { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public List<Guid> ProcessIds { get; set; } = [];
    
    public HardwareBriefInfo HardwareInfo { get; set; } = null!;
    
    public List<NetworkConnection> Connections { get; set; } = [];

    public Dictionary<string, string> WindowsRegistry { get; set; } = [];
    
    public bool IsDockerEnabled { get; set; }

    public List<RunningDockerContainer> DockerContainers { get; set; } = [];
}
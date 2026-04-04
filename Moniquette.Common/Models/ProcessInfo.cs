namespace Moniquette.Common.Models;

public class ProcessInfo
{
    public int Pid { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public string ExecutablePath { get; set; } = string.Empty;

    public long[]? Signature { get; set; }
}
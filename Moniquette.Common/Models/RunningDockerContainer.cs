namespace Moniquette.Common.Models;

public class RunningDockerContainer
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ImageName { get; set; } = null!;
}
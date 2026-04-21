using Moniquette.Common.Models;

namespace Moniquette.Client.Services.Abstractions;

public interface IDockerService
{
    public bool CheckDockerIsRunning();
    
    public  List<RunningDockerContainer> GetRunningDockerContainers();
}
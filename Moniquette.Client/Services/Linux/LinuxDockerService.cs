using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Services.Linux;

public class LinuxDockerService : IDockerService
{
    private const string CheckDockerLinuxCommand = "systemctl is-active docker";

    private const string GetContainersLinuxCommand =
        """
        docker ps --format '{{.Names}}\t{{.Image}}' | while IFS='	' read -r name imagename; do
          digest=$(docker image inspect --format='{{index .RepoDigests 0}}' "$imagename" 2>/dev/null)
          printf '%s\t%s\t%s\n' "$name" "$imagename" "${digest:-}"
        done
        """;

    public bool CheckDockerIsRunning()
    {
        var processResult = LinuxUtils.RunShellScript(CheckDockerLinuxCommand);
        return processResult && processResult.Value.Equals("active");
    }

    public List<RunningDockerContainer> GetRunningDockerContainers()
    {
        var processResult = LinuxUtils.RunShellScript(GetContainersLinuxCommand);
        if (!processResult)
        {
            throw new Exception(
                $"Couldn't execute the command '{GetContainersLinuxCommand}'{Environment.NewLine}"
                + processResult.Error);
        }

        var output = processResult.Value.Trim();
        return output.Equals(string.Empty)
            ? []
            : output
                .Split(Environment.NewLine)
                .Select(line =>
                {
                    var info = line.Split('\t');
                    return new RunningDockerContainer
                    {
                        Name = info.ElementAtOrDefault(0) ?? string.Empty,
                        ImageName = info.ElementAtOrDefault(1) ?? string.Empty,
                        ImageDigest = info.ElementAtOrDefault(2) ?? string.Empty
                    };
                })
                .Where(container => !string.IsNullOrWhiteSpace(container.Name)
                                    && !string.IsNullOrWhiteSpace(container.ImageName))
                .ToList();
    }
}

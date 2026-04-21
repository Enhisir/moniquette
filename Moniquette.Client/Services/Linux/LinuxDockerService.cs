using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Services.Linux;

public class LinuxDockerService : IDockerService
{
    private const string CheckDockerLinuxCommand = "systemctl is-active docker";

    private const string GetContainersLinuxCommand =
        """
        docker ps -q | while read cid; do
          name=$(docker inspect --format='{{.Name}}' "$cid" | sed 's/^\/\(.*\)/\1/')
          imagename=$(docker inspect --format='{{.Config.Image}}' "$cid")
          digest=$(docker inspect --format='{{index .RepoDigests 0}}' "$imagename" 2>/dev/null)
          echo -e "$name\t$imagename\t${digest:-<no-digest>}"
        done | column -t
        """;

    // TODO: Заменить
    /*
     * docker ps -q | while read cid; do
         name=$(docker inspect --format='{{.Name}}' "$cid" | sed 's/^\/\(.*\)/\1/')
         imagename=$(docker inspect --format='{{.Config.Image}}' "$cid")
         digest=$(docker inspect --format='{{index .RepoDigests 0}}' "$imagename" 2>/dev/null)
         echo -e "$name\t$imagename\t${digest:-<no-digest>}"
       done | column -t
     */

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
                    var info = line.Trim().Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    return new RunningDockerContainer
                    {
                        ImageDigest = info[0],
                        Name = info[1],
                        ImageName = info[2]
                    };
                })
                .ToList();
    }
}
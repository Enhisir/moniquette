using System.Diagnostics;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;

namespace Moniquette.Client.Services.Windows;

public class WindowsDockerService : IDockerService
{
    private const string CheckDockerWindowsCommand = "Get-Process 'com.docker.proxy'";

    private const string GetContainersWindowsCommand =
        """
        docker ps --format "{{.Names}}`t{{.Image}}" | ForEach-Object {
          $parts = $_ -split "`t", 2
          $digest = ""
          try {
            $digest = docker image inspect --format "{{index .RepoDigests 0}}" $parts[1] 2>$null
          } catch {}
          "$($parts[0])`t$($parts[1])`t$digest"
        }
        """;

    public bool CheckDockerIsRunning()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -Command \"{CheckDockerWindowsCommand}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            proc?.WaitForExit(3000);
            return proc?.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<RunningDockerContainer> GetRunningDockerContainers()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-ExecutionPolicy Bypass -Command \"{GetContainersWindowsCommand}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = Process.Start(psi)
                         ?? throw new InvalidOperationException("Could not start docker ps process.");
        var output = proc.StandardOutput.ReadToEnd();
        var error = proc.StandardError.ReadToEnd();
        proc.WaitForExit(5000);

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException($"Could not collect Docker containers: {error}");
        }

        return output
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split('\t'))
            .Select(info => new RunningDockerContainer
            {
                Name = info.ElementAtOrDefault(0) ?? string.Empty,
                ImageName = info.ElementAtOrDefault(1) ?? string.Empty,
                ImageDigest = info.ElementAtOrDefault(2) ?? string.Empty
            })
            .Where(container => !string.IsNullOrWhiteSpace(container.Name)
                                && !string.IsNullOrWhiteSpace(container.ImageName))
            .ToList();
    }
}

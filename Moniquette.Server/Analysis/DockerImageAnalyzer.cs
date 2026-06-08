using Moniquette.Common.Enums;
using Moniquette.Common.Models;
using Moniquette.Server.Repositories;

namespace Moniquette.Server.Analysis;

public class DockerImageAnalyzer(
    ISuspiciousDockerImageRepository suspiciousDockerImageRepository) : AnalyzerBase
{
    public override async Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        if (!report.IsDockerEnabled || report.DockerContainers.Count == 0)
        {
            return [];
        }

        var threats = new List<Threat>();
        foreach (var container in report.DockerContainers)
        {
            var matches = await suspiciousDockerImageRepository.FindMatchesAsync(container, cancellationToken);
            var match = matches.FirstOrDefault();
            if (match is null)
            {
                continue;
            }

            var reason = string.IsNullOrWhiteSpace(match.Details)
                ? "образ найден в каталоге подозрительных Docker images"
                : match.Details;

            threats.Add(CreateThreat(
                report,
                ThreatType.Threat,
                $"Docker-контейнер {container.Name} использует подозрительный образ {container.ImageName}: {reason}."));
        }

        return threats;
    }
}

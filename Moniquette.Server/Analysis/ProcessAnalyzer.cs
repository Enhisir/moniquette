using Moniquette.Common.Enums;
using Moniquette.Common.Models;
using Moniquette.Elastic.Services;
using Moniquette.Server.Repositories;

namespace Moniquette.Server.Analysis;

public class ProcessAnalyzer(
    IProcessBandService processBandService,
    ISuspiciousProcessRepository suspiciousProcessRepository) : AnalyzerBase
{
    public override async Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var platform = DetectPlatform(report);
        var threats = new List<Threat>();

        foreach (var process in report.Processes)
        {
            var bands = processBandService.CreateBands(process.Signature);
            if (bands.Length == 0)
            {
                continue;
            }

            var candidates = await suspiciousProcessRepository.FindByBandsAsync(
                bands,
                platform,
                limit: 20,
                cancellationToken);

            var match = candidates
                .Select(candidate => new
                {
                    Process = candidate,
                    Coincidence = CalculateCoincidence(bands, candidate.Bands)
                })
                .Where(match => match.Coincidence >= 0.6)
                .OrderByDescending(match => match.Coincidence)
                .FirstOrDefault();

            if (match is null)
            {
                continue;
            }

            threats.Add(CreateThreat(
                report,
                ThreatType.Threat,
                $"Процесс {process.Name} похож на подозрительный бинарник {match.Process.Name}: совпало {Math.Round(match.Coincidence * 100, 1)}% бэндов."));
        }

        return threats;
    }

    private static ProcessCatalogPlatform DetectPlatform(Report report)
        => ContainsAny(report.HardwareInfo.OperatingSystem, "windows")
            ? ProcessCatalogPlatform.Windows
            : ProcessCatalogPlatform.Linux;

    private static double CalculateCoincidence(long[] bands, long[] candidateBands)
    {
        if (bands.Length == 0 || candidateBands.Length == 0)
        {
            return 0;
        }

        var candidateSet = candidateBands.ToHashSet();
        var equal = bands.Count(candidateSet.Contains);
        return equal / (double)bands.Length;
    }
}

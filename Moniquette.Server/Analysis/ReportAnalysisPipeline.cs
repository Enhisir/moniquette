using Moniquette.Common.Models;
using Moniquette.Server.Repositories;

namespace Moniquette.Server.Analysis;

public interface IReportAnalysisPipeline
{
    Task<IReadOnlyCollection<Threat>> AnalyzeAndSaveAsync(Report report, CancellationToken cancellationToken);
}

public class ReportAnalysisPipeline(
    IEnumerable<IReportAnalyzer> analyzers,
    IThreatRepository threatRepository,
    ILogger<ReportAnalysisPipeline> logger) : IReportAnalysisPipeline
{
    public async Task<IReadOnlyCollection<Threat>> AnalyzeAndSaveAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var threats = new List<Threat>();

        foreach (var analyzer in analyzers)
        {
            try
            {
                threats.AddRange(await analyzer.AnalyzeAsync(report, cancellationToken));
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Report analyzer {AnalyzerName} failed for report {ReportId}.",
                    analyzer.GetType().Name,
                    report.Id);
            }
        }

        if (threats.Count > 0)
        {
            await threatRepository.SaveManyAsync(threats, cancellationToken);
        }

        return threats;
    }
}

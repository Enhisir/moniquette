using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public interface IReportAnalyzer
{
    Task<IReadOnlyCollection<Threat>> AnalyzeAsync(Report report, CancellationToken cancellationToken);
}

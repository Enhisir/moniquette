using Moniquette.Common.Enums;
using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public abstract class AnalyzerBase : IReportAnalyzer
{
    public abstract Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken);

    protected static Threat CreateThreat(Report report, ThreatType type, string details)
        => new()
        {
            Type = type,
            SessionId = report.SessionId,
            ReportId = report.Id,
            Details = details
        };

    protected static bool ContainsAny(string? value, params string[] markers)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return markers.Any(marker =>
            value.Contains(marker, StringComparison.OrdinalIgnoreCase));
    }
}

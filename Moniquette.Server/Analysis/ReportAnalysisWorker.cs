using Moniquette.Common.Models;
using Moniquette.Server.Hubs;

namespace Moniquette.Server.Analysis;

public class ReportAnalysisWorker(
    IReportAnalysisQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<ReportAnalysisWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var report in queue.ReadAllAsync(stoppingToken))
        {
            await AnalyzeReportAsync(report, stoppingToken);
        }
    }

    private async Task AnalyzeReportAsync(Report report, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var pipeline = scope.ServiceProvider.GetRequiredService<IReportAnalysisPipeline>();
            var notifier = scope.ServiceProvider.GetRequiredService<IMonitoringNotifier>();
            var threats = await pipeline.AnalyzeAndSaveAsync(report, cancellationToken);
            await notifier.NotifyReportAnalyzedAsync(report, threats, cancellationToken);

            logger.LogInformation(
                "Report {ReportId} for session {SessionId} analyzed from queue. Threats: {ThreatCount}.",
                report.Id,
                report.SessionId,
                threats.Count);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to analyze queued report {ReportId} for session {SessionId}.",
                report.Id,
                report.SessionId);
        }
    }
}

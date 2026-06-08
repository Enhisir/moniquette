using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moniquette.Client.Pipeline.Fillers;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline;

public class ReportPipeline(IServiceProvider provider)
{
    private ILogger<ReportPipeline> Logger { get; } =
        provider.GetService<ILogger<ReportPipeline>>()
        ?? throw new InvalidOperationException("Missing ReportPipeline logger.");

    public async Task<Report> RunAsync(Guid sessionId, CancellationToken ct = default)
    {
        var report = new Report
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Timestamp = DateTime.UtcNow
        };

        return await provider
            .GetServices<IReportFiller>()
            .ToAsyncEnumerable()
            .AggregateAwaitWithCancellationAsync(report, ApplyFillerAsync, ct);
    }
    
    private async ValueTask<Report> ApplyFillerAsync(
        Report accumulate, 
        IReportFiller filler, 
        CancellationToken ct = default)
    {
        try
        {
            return await filler.Fill(accumulate, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(
                ex,
                "Report filler {FillerName} failed. The report will be sent with partial data.",
                filler.GetType().Name);
            return accumulate;
        }
    }
}

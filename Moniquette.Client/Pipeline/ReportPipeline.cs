using Microsoft.Extensions.DependencyInjection;
using Moniquette.Client.Pipeline.Fillers;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline;

public class ReportPipeline(IServiceProvider provider)
{
    public async Task<Report> RunAsync(Guid sessionId, CancellationToken ct = default)
    {
        var report = new Report { SessionId = sessionId, Timestamp = DateTime.UtcNow };

        return await provider
            .GetServices<IReportFiller>()
            .ToAsyncEnumerable()
            .AggregateAwaitWithCancellationAsync(report, ApplyFillerAsync, ct);
    }
    
    private static async ValueTask<Report> ApplyFillerAsync(
        Report accumulate, 
        IReportFiller filler, 
        CancellationToken ct = default)
        => await filler.Fill(accumulate, ct);
}
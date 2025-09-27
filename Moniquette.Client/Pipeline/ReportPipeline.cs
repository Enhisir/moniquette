using Microsoft.Extensions.DependencyInjection;
using Moniquette.Client.Pipeline.Fillers;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline;

public class ReportPipeline(IServiceProvider provider)
{
    public async Task<Report> RunAsync(CancellationToken ct = default)
    {
        var report = new Report();
        var fillers = provider.GetServices<IReportFiller>();

        foreach (var filler in fillers)
        {
            report = await filler.Fill(report, ct);
        }

        return report;
    }
}
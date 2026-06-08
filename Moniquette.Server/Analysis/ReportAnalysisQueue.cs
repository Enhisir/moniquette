using System.Threading.Channels;
using Moniquette.Common.Models;

namespace Moniquette.Server.Analysis;

public interface IReportAnalysisQueue
{
    ValueTask EnqueueAsync(Report report);

    IAsyncEnumerable<Report> ReadAllAsync(CancellationToken cancellationToken);
}

public class ReportAnalysisQueue : IReportAnalysisQueue
{
    private readonly Channel<Report> channel = Channel.CreateUnbounded<Report>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(Report report)
        => channel.Writer.WriteAsync(report);

    public IAsyncEnumerable<Report> ReadAllAsync(CancellationToken cancellationToken)
        => channel.Reader.ReadAllAsync(cancellationToken);
}

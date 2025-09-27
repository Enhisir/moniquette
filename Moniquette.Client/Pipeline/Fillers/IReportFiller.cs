using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public interface IReportFiller
{
    public Task<Report> Fill(Report request, CancellationToken cancellationToken);
}
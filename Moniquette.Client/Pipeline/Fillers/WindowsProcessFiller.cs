using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public class WindowsProcessFiller : IReportFiller
{
    public Task<Report> Fill(Report request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
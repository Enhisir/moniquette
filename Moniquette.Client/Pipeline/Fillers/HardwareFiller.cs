using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public class HardwareFiller(Hardware.Info.IHardwareInfo info) : IReportFiller
{
    public Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        report.HardwareInfo = HardwareBriefInfo.FromFullInfo(info);
        return Task.FromResult(report);
    }
}
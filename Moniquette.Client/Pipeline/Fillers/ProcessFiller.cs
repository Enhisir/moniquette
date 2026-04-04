using Gee.External.Capstone.X86;
using Moniquette.Common.Models;
using Moniquette.ProcessObserver.ProcessPipeline;
using Moniquette.ProcessObserver.Services;

namespace Moniquette.Client.Pipeline.Fillers;

public class ProcessFiller(
    CapstoneX86Disassembler capstone,
    BinarySignatureManager bsc) : IReportFiller
{
    public async Task<Report> Fill(Report request, CancellationToken cancellationToken)
    {
        var pipeline = new ProcessPipeline(capstone, bsc);
        request.Processes = pipeline.GetWithSignature();
        return await Task.FromResult(request);
    }
}
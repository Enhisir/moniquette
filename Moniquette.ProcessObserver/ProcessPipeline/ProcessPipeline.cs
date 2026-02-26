using Gee.External.Capstone.X86;
using Moniquette.ProcessObserver.Models;
using Moniquette.ProcessObserver.Services;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public class ProcessPipeline(
    CapstoneX86Disassembler capstone,
    BinarySignatureManager psc)
{
    private List<IProcessHandler> Handlers { get; } =
    [
        new FlatpakProcessHandler(),
        new LinuxProcessHandler(),
        new ProcessSignatureHandler(capstone, psc)
    ];

    public List<ProcessInfo> GetWithSignature()
    {
        return Handlers
            .Aggregate(
                Enumerable.Empty<ProcessInfo>(),
                (sum, handler) => handler.Invoke(sum))
            .ToList();
    }
}
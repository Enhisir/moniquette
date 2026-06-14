using Gee.External.Capstone.X86;
using Moniquette.Common.Models;
using Moniquette.ProcessObserver.Services;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public class ProcessPipeline(
    CapstoneX86Disassembler capstone,
    BinarySignatureManager psc)
{
    private List<IProcessHandler> Handlers { get; } =
    [
        ..CreateProcessHandlers(),
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

    private static IEnumerable<IProcessHandler> CreateProcessHandlers()
    {
        if (OperatingSystem.IsWindows())
        {
            return [new WindowsProcessHandler()];
        }

        if (OperatingSystem.IsLinux())
        {
            return [new FlatpakProcessHandler(), new LinuxProcessHandler()];
        }

        return [];
    }
}

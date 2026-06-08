using Gee.External.Capstone.X86;
using Moniquette.Common.Models;
using Moniquette.ProcessObserver.Services;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public class ProcessSignatureHandler(
    CapstoneX86Disassembler capstone,
    BinarySignatureManager psc) : IProcessHandler
{
    public IEnumerable<ProcessInfo> Invoke(IEnumerable<ProcessInfo>? processInfos = null)
    {
        var bfe = new BinaryFileExtractor(capstone);
        foreach (var pInfo in processInfos ?? [])
        {
            var successful = true;
            try
            {
                var instructions = bfe.GetTextSegmentInstructions(pInfo.ExecutablePath);
                pInfo.Signature = psc.CreateSignatureX86(instructions);
            }
            catch (Exception ex)
            {
                successful = false;
                Console.Error.WriteLine(
                    $"Failed to create process signature for '{pInfo.ExecutablePath}': {ex.Message}");
            }
            if (successful)
                yield return pInfo;
        }
    }
}

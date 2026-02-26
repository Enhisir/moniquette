using Gee.External.Capstone.X86;
using Moniquette.ProcessObserver.Models;
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
                // write exception handling
            }
            if (successful)
                yield return pInfo;
        }
    }
}
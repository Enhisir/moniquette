using Moniquette.ProcessObserver.Models;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public interface IProcessHandler
{
    public IEnumerable<ProcessInfo> Invoke(
        IEnumerable<ProcessInfo>? processInfos = null);
}
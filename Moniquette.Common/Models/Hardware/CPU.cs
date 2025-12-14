namespace Moniquette.Common.Models.Hardware;

public class CPU
{
    public string Name { get; set; }
    public uint Cores { get; set; }
    public uint NumberOfLogicalProcessors { get; set; }
    
    public uint L1InstructionCacheSize { get; set; } // сайзы должны сопоставляться с аналогичными значениям из БД
    public uint L1DataCacheSize { get; set; }
    public uint L2CacheSize { get; set; }
    public uint L3CacheSize { get; set; }

}
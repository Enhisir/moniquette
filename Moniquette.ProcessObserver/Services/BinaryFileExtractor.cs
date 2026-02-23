using ELFSharp.ELF;
using Gee.External.Capstone.X86;

namespace Moniquette.ProcessObserver.Services;

public class BinaryFileExtractor(CapstoneX86Disassembler capstone)
{
    private const int DefaultMaxInstructions = 100_000;
    
    public X86Instruction[] GetTextSegmentInstructions(string executablePath)
    {
        using var elf = ELFReader.Load(executablePath);
        var textBytes = elf.GetSection(".text").GetContents();
        return capstone.Disassemble(textBytes, 0, DefaultMaxInstructions);
    }
}
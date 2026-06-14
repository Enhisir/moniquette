using ELFSharp.ELF;
using Gee.External.Capstone.X86;
using System.Reflection.PortableExecutable;

namespace Moniquette.ProcessObserver.Services;

public class BinaryFileExtractor(CapstoneX86Disassembler capstone)
{
    private const int DefaultMaxInstructions = 100_000;
    
    public X86Instruction[] GetTextSegmentInstructions(string executablePath)
    {
        if (OperatingSystem.IsWindows())
        {
            return GetPortableExecutableInstructions(executablePath);
        }

        return GetElfInstructions(executablePath);
    }

    private X86Instruction[] GetElfInstructions(string executablePath)
    {
        using var elf = ELFReader.Load(executablePath);
        var textBytes = elf.GetSection(".text").GetContents();
        return capstone.Disassemble(textBytes, 0, DefaultMaxInstructions);
    }

    private X86Instruction[] GetPortableExecutableInstructions(string executablePath)
    {
        using var stream = File.OpenRead(executablePath);
        using var peReader = new PEReader(stream);
        var codeBytes = peReader.PEHeaders.SectionHeaders
            .Where(IsExecutableCodeSection)
            .SelectMany(section => peReader
                .GetSectionData(section.VirtualAddress)
                .GetContent()
                .ToArray())
            .ToArray();

        if (codeBytes.Length == 0)
        {
            throw new InvalidOperationException("Portable executable does not contain executable code sections.");
        }

        return capstone.Disassemble(codeBytes, 0, DefaultMaxInstructions);
    }

    private static bool IsExecutableCodeSection(SectionHeader section)
        => section.SectionCharacteristics.HasFlag(SectionCharacteristics.ContainsCode)
           || section.SectionCharacteristics.HasFlag(SectionCharacteristics.MemExecute)
           || section.Name.Equals(".text", StringComparison.OrdinalIgnoreCase);
}

using System.CommandLine;
using System.Text.Json;
using Gee.External.Capstone;
using Gee.External.Capstone.X86;
using Moniquette.ProcessObserver.Infrastructure;
using Moniquette.ProcessObserver.Services;

namespace Moniquette.ProcessObserver;

internal static class Program
{
    static Program()
    {
        Capstone.EnableInstructionDetails = true;
    }

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Moniquette Binary Observer & Comparer");

        var compareCommand = new Command("compare", "Compare 2 executable files");
        rootCommand.Subcommands.Add(compareCommand);
        Argument<FileInfo> firstBinary = new("first-file");
        compareCommand.Arguments.Add(firstBinary);
        Argument<FileInfo> secondBinary = new("second-file");
        compareCommand.Arguments.Add(secondBinary);
        compareCommand.SetAction(result => Compare(
            result.GetValue(firstBinary)!.FullName,
            result.GetValue(secondBinary)!.FullName));

        var observeProcessesCommand = new Command("ps", "Observe binary processes and create their signatures");
        rootCommand.Subcommands.Add(observeProcessesCommand);
        var observeProcessesOutputParameter = new Argument<FileInfo>("output")
        {
            DefaultValueFactory = _ => new FileInfo(Path.Combine(Environment.CurrentDirectory, "output.json"))
        };
        observeProcessesCommand.Arguments.Add(observeProcessesOutputParameter);
        observeProcessesCommand.SetAction(result =>
            Observe(result.GetValue(observeProcessesOutputParameter)!.FullName));

        return rootCommand.Parse(args).Invoke();
    }

    private static void Compare(string executablePathOne, string executablePathTwo)
    {
        var extractor = new BinaryFileExtractor(Capstone);

        var instructionsOne = extractor.GetTextSegmentInstructions(executablePathOne);
        var signatureOne = Bsc.CreateSignatureX86(instructionsOne);

        var instructionsTwo = extractor.GetTextSegmentInstructions(executablePathTwo);
        var signatureTwo = Bsc.CreateSignatureX86(instructionsTwo);

        var comparer = new BinarySignatureComparer();
        var result = comparer.Compare(signatureOne, signatureTwo);
        Console.WriteLine("Signatures coincidence: {0} %", Math.Round(result, 4) * 100);
    }

    private static void Observe(string outputPath)
    {
        var pipeline = new ProcessPipeline.ProcessPipeline(Capstone, Bsc);
        var pInfos = pipeline.GetWithSignature();

        using var fileStream = new FileStream("output.json", FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(fileStream, pInfos, JsonSerializerOptions.Default);
        Console.WriteLine("Report saved into {0}", outputPath);
    }

    private static readonly CapstoneX86Disassembler Capstone =
        CapstoneDisassembler.CreateX86Disassembler(X86DisassembleMode.Bit64);

    private static readonly BinarySignatureManager Bsc = new(Config.Instance);
}
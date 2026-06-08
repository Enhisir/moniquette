using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;

namespace Moniquette.Elastic.Services;

public interface IProcessBandService
{
    long[] CreateBands(long[]? signature);

    ElasticReportProcess CreateReportProcess(ProcessInfo process, Guid sessionId, Guid reportId, DateTime timestamp);
}

public class ProcessBandService : IProcessBandService
{
    public const int SignatureSize = 400;
    public const int BandCount = 20;
    private const int BandSize = SignatureSize / BandCount;

    public long[] CreateBands(long[]? signature)
    {
        if (signature is null || signature.Length == 0)
        {
            return [];
        }

        if (signature.Length != SignatureSize)
        {
            throw new ArgumentException(
                $"Process signature must contain exactly {SignatureSize} numbers.",
                nameof(signature));
        }

        var bands = new long[BandCount];
        for (var bandIndex = 0; bandIndex < BandCount; bandIndex++)
        {
            var offset = bandIndex * BandSize;
            bands[bandIndex] = CreateBandHash(signature, offset);
        }

        return bands;
    }

    public ElasticReportProcess CreateReportProcess(
        ProcessInfo process,
        Guid sessionId,
        Guid reportId,
        DateTime timestamp)
    {
        var signature = process.Signature ?? [];
        return new ElasticReportProcess
        {
            SessionId = sessionId,
            ReportId = reportId,
            Timestamp = timestamp,
            Pid = process.Pid,
            Name = process.Name,
            Title = process.Title,
            ExecutablePath = process.ExecutablePath,
            Signature = signature,
            Bands = CreateBands(signature)
        };
    }

    private static long CreateBandHash(long[] signature, int offset)
    {
        unchecked
        {
            const ulong fnvOffsetBasis = 14695981039346656037UL;
            const ulong fnvPrime = 1099511628211UL;

            var hash = fnvOffsetBasis;
            for (var i = offset; i < offset + BandSize; i++)
            {
                hash ^= (ulong)signature[i];
                hash *= fnvPrime;
            }

            return (long)hash;
        }
    }
}

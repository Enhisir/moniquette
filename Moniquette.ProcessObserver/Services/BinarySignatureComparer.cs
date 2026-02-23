namespace Moniquette.ProcessObserver.Services;

public class BinarySignatureComparer
{
    public double Compare(long[] signatureA, long[] signatureB)
    {
        if (signatureA.Length != signatureB.Length
            || signatureA.Length == 0) return 0.0;

        var equal = signatureA.Where((t, i) => t == signatureB[i]).Count();
        return equal / (double)signatureA.Length;
    }
}
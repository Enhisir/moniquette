using Gee.External.Capstone.X86;
using Moniquette.ProcessObserver.Infrastructure;

namespace Moniquette.ProcessObserver;

public class BinarySignatureManager(Config config)
{
    private readonly List<HashFunction> _hashFunctions = config
        .Seeds
        .Select(CreateHashFunction)
        .ToList();
    
    public long[] CreateSignatureX86(IEnumerable<X86Instruction> instructions)
    {
        var minHashes = Enumerable.Repeat(long.MaxValue, _hashFunctions.Count).ToArray();

        var tokenBuilderIndex = 0;
        var tokenBuilder = new char[16]; // оптимизация записи токена
        foreach (var inst in instructions)
        {
            foreach (var c in inst.Mnemonic)
                tokenBuilder[tokenBuilderIndex++] = c;
            
            foreach (var op in inst.Details?.Operands ?? [])
                tokenBuilder[tokenBuilderIndex++] = (char)op.Type;

            for (var i = 0; i < _hashFunctions.Count; i++)
            {
                var s = new string(tokenBuilder, 0, tokenBuilderIndex); // соптимизовать 
                var h = _hashFunctions[i](s);
                if (h < minHashes[i]) minHashes[i] = h;
            }

            tokenBuilderIndex = 0;
        }

        return minHashes;
    }
    
    /// <summary>
    /// Функция используется только
    /// для генерации изначального массива сидов, 
    /// который впоследствии записывается в конфиг
    /// </summary>
    /// <param name="maxError">Уровень значимости</param>
    /// <returns>
    /// Массив сидов, состоящий из положительных целых чисел 
    /// в диапазоне от 32 до (32 + 1024 * 1 / maxError ^ 2)
    /// </returns>
    public static long[] GetSeeds(double maxError = 0.05)
    {
        var size = double.ConvertToInteger<int>(Math.Round(1 / (maxError * maxError)));
        var seeds = new long[size];
        for (var i = 0; i < size; ++i)
        {
            var r = Math.Abs(Math.Floor((double)Random.Shared.Next() % 1024 * size));
            seeds[i] = Convert.ToInt64(r) + 32L;
        }

        return seeds;
    }

    private static HashFunction CreateHashFunction(long seed)
    {
        return s =>
        {
            var hash = (uint)seed;
            return s.Aggregate(hash, (current, c) => (current * 16777619) ^ c);
        };
    }
}

public delegate long HashFunction(string input);
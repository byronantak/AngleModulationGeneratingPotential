using System.Collections.Generic;
using System.Linq;

namespace HonoursGeneratingFunction.GeneratingFunction
{
    public static class BitStringDictionaryProvider
    {
        private static readonly string bitStringAlphabet = "01";

        public static Dictionary<string, int> CreateBitStringDictionary(int bitStringSize, int startValue = 0)
        {
            var allBitStrings = BitStringDictionaryProvider.GeneratePermuations(bitStringAlphabet, bitStringSize);
            return allBitStrings.ToDictionary(x => x, x => startValue);
        }

        public static IEnumerable<string> GeneratePermuations(string alphabet, int numberPermutations)
        {
            var q = alphabet.Select(x => x.ToString());
            int numberOfPairs = numberPermutations - 1;
            for (var i = 0; i < numberOfPairs; i++)
            {
                q = q.SelectMany(x => alphabet, (x, y) => x + y);
            }
            return q;
        }
    }
}

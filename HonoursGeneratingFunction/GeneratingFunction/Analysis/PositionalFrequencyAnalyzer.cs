using System.Collections.Generic;
using System.Linq;

namespace HonoursGeneratingFunction.GeneratingFunction.Analysis
{
    public static class PositionalFrequencyAnalyzer
    {
        public static IEnumerable<double> CalculateFrequencyOfOneForEachPosition(Dictionary<string, double> bitStringLookup)
        {
            var bitStringLength = bitStringLookup.Keys.First().Length;
            var totalStrings = bitStringLookup.Values.Sum();
            var positionOneCount = Enumerable.Repeat(0d, bitStringLength).ToArray();
            foreach (var keyValue in bitStringLookup)
            {
                var bitString = keyValue.Key;
                var bitStringCount = keyValue.Value;
                for(var i = 0; i < bitStringLength; i++)
                {
                    var bit = bitString.ElementAt(i);
                    if (bit == '1')
                    {
                        positionOneCount[i] += bitStringCount;
                    }
                }
            }

            for (var i = 0; i < bitStringLength; i++)
            {
                positionOneCount[i] /= totalStrings;
            }

            return positionOneCount.ToList();
        }
    }
}

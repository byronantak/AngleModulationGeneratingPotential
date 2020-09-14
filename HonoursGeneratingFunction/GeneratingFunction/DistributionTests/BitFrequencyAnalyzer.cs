using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HonoursGeneratingFunction.GeneratingFunction.DistributionTests
{
    public static class BitFrequencyAnalyzer
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        public const string FilePath = @"C:\Users\byron.antak\source\repos\HonoursGeneratingFunction\GeneratingFunction\PreviousResults\Results.json";
#pragma warning restore S1075 // URIs should not be hardcoded

        public static void Analyze()
        {
            var jsonContent = File.ReadAllText(FilePath);
            var bitStringLookup = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonContent);
            var totalStringsProduced = Math.Pow(81, 4);
            var bitStringLength = bitStringLookup.Keys.First().Length;

            var oneCount = new int[bitStringLength];
            foreach(var keyValue in bitStringLookup)
            {
                var bitString = keyValue.Key;
                for (var i = 0; i < bitStringLength; i++)
                {
                    oneCount[i] += GetOneCount(bitString, i, keyValue.Value);
                }
            }

            var frequencies = new double[bitStringLength];
            for(var i = 0; i < bitStringLength; i++) {
                frequencies[i] = oneCount[i] / totalStringsProduced;
                Console.WriteLine($"Position: {i} 1 Percentage => {frequencies[i]}");
            }

        }

        public static List<double> GetFreqs()
        {
            var jsonContent = File.ReadAllText(FilePath);
            var bitStringLookup = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonContent);
            var totalStringsProduced = Math.Pow(81, 4);
            return bitStringLookup.Values.Select(x => x / totalStringsProduced).ToList();
        }

        public static List<double> GetValues()
        {
            var jsonContent = File.ReadAllText(FilePath);
            var bitStringLookup = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonContent);
            return bitStringLookup.Values.ToList();
        }

        private static int GetOneCount(string bitString, int index, int count)
        {
            if (bitString[index] == '1')
            {
                return count;
            }

            return 0;
        }

    }
}

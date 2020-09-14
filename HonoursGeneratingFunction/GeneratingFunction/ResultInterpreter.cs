using HonoursGeneratingFunction.GeneratingFunction.Analysis;
using HonoursGeneratingFunction.GeneratingFunction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HonoursGeneratingFunction.GeneratingFunction
{
    public static class ResultInterpreter
    {
        public const string HAMMING_FREQUENCIES_FILE_PATH = "hammingFrequencies.json";
        public const string RUNS_FILE_PATH = "runs.json";
        public const string FREQUENCY_FILE_PATH = "frequencies.json";
        public const string REFERENCE_HAMMING_STRING = "0000000000000000";

        public static void Interpret(int bitStringLength, string resultFilePath)
        {
            var savePath = Path.Join(resultFilePath, "Results");
            var countsFilePath = Path.Join(savePath, GeneratingPotential.COUNTS_FILE_NAME);
            var jsonContent = File.ReadAllText(countsFilePath);
            var bitStringLookup = JsonSerializer.Deserialize<Dictionary<string, double>>(jsonContent);
            HammingFrequencyAnalysis(bitStringLookup, bitStringLength, savePath);
            LongestRunAnalysis(bitStringLookup, bitStringLength, savePath);
            ConditionalProbabilityAnalyzer.PerformConditionalProbabilityAnalysis(bitStringLookup, savePath);
            ConditionalProbabilitySummarizer.SummarizeProbabilities(bitStringLookup, savePath);
            StandardAnalysis(bitStringLookup, savePath);
        }

        private static List<ConditionalProbabilityStat> CalculateOverallConditionalProbability(Dictionary<string, double> bitStringLookup, int bitStringLength, double totalStrings)
        {
            var conditionalProbDict = new Dictionary<string, double>();
            foreach (var i in Enumerable.Range(0, bitStringLength - 1))
            {
                foreach (var keyValue in bitStringLookup)
                {
                    foreach (var bit in Enumerable.Range(0, 2))
                    {
                        foreach (var nextBit in Enumerable.Range(0, 2))
                        {
                            var myKey = GetConditionalProbKey(i, bit, nextBit);
                            conditionalProbDict[myKey] = 0;
                        }
                    }
                }
            }

            foreach (var i in Enumerable.Range(0, bitStringLength - 1))
            {
                foreach (var keyValue in bitStringLookup)
                {
                    var currentBit = keyValue.Key.ElementAt(i) == '1' ? 1 : 0;
                    var nextBit = keyValue.Key.ElementAt(i + 1) == '1' ? 1 : 0;
                    var myKey = GetConditionalProbKey(i, currentBit, nextBit);
                    conditionalProbDict[myKey] += keyValue.Value;
                }
            }

            var otherDict = conditionalProbDict.ToDictionary(x => x.Key, x => x.Value / totalStrings);
            var conditionalProbabilityByPositions = new ConditionalProbabilityStat[bitStringLength];

            foreach (var keyValue in otherDict)
            {
                foreach (var i in Enumerable.Range(0, bitStringLength - 1))
                {
                    if (keyValue.Key.Contains($"AtPosition{i}&"))
                    {
                        if (conditionalProbabilityByPositions[i] == null)
                        {
                            conditionalProbabilityByPositions[i] = new ConditionalProbabilityStat
                            {
                                Position = i,
                                ConditionalProbDictionary = new Dictionary<string, double>()
                                {
                                    { keyValue.Key, keyValue.Value }
                                }
                            };
                        }
                        else
                        {
                            conditionalProbabilityByPositions[i].ConditionalProbDictionary[keyValue.Key] = keyValue.Value;
                        }
                    }

                }

            }

            return conditionalProbabilityByPositions.Where(x => x != null).OrderBy(x => x.Position).ToList();
        }

        private static Dictionary<string, double> GetHammingFrequencies(Dictionary<string, double> bitStringLookup, int bitStringLength, double totalStrings)
        {
            var hammingDistanceCounts = new Dictionary<int, long>();
            foreach (var hamDist in Enumerable.Range(0, bitStringLength + 1))
            {
                hammingDistanceCounts[hamDist] = 0;
            }

            foreach (var keyValue in bitStringLookup)
            {
                var bitString = keyValue.Key;
                var bitStringCount = keyValue.Value;
                var hammingDistance = GetHammingDistance(bitString, REFERENCE_HAMMING_STRING);
                hammingDistanceCounts[hammingDistance] += (long)bitStringCount;
            }

            return hammingDistanceCounts.OrderByDescending(x => x.Value).ToDictionary(x => x.Key.ToString(), x => (double)x.Value / totalStrings);
        }

        private static void HammingFrequencyAnalysis(Dictionary<string, double> bitStringLookup, int bitStringLength, string savePath)
        {
            var totalOnesProduced = 0d;
            foreach (var keyValue in bitStringLookup)
            {
                var bitString = keyValue.Key;
                var bitStringCount = keyValue.Value;
                var oneCount = bitString.Count(x => x == '1');
                totalOnesProduced += oneCount * bitStringCount;
            }
            var totalStrings = bitStringLookup.Values.Sum();

            var adhocStats = new AdHocStats
            {
                FrequencyOfOnesProduced = totalOnesProduced / (totalStrings * bitStringLength),
                HammingFrequency = GetHammingFrequencies(bitStringLookup, bitStringLength, totalStrings),
                ConditionalProbability = CalculateOverallConditionalProbability(bitStringLookup, bitStringLength, totalStrings)
            };

            var jsonString = JsonSerializer.Serialize(adhocStats);
            File.WriteAllText(Path.Join(savePath, HAMMING_FREQUENCIES_FILE_PATH), jsonString);
        }

        private static void LongestRunAnalysis(Dictionary<string, double> bitStringLookup, int bitStringLength, string savePath)
        {
            var dictionaryLongestRuns = new Dictionary<string, double>();
            for (var i = 0; i < bitStringLength + 1; i++)
            {
                dictionaryLongestRuns[i.ToString()] = 0;
            }
            foreach (var keyValue in bitStringLookup)
            {
                var bitString = keyValue.Key;
                var bitStringCount = keyValue.Value;
                var longestRun = GetLongestRunOfOnes(bitString);
                dictionaryLongestRuns[longestRun.ToString()] += bitStringCount;
            }
            var jsonString = JsonSerializer.Serialize(dictionaryLongestRuns);
            File.WriteAllText(Path.Join(savePath, RUNS_FILE_PATH), jsonString);
        }

        private static int GetLongestRunOfOnes(string s)
        {
            var longestRun = 0;
            var currentRun = 0;
            char? previousChar = null;
            foreach (var bit in s)
            {
                if (!previousChar.HasValue || previousChar == bit)
                {
                    currentRun++;
                    if (currentRun > longestRun)
                    {
                        longestRun = currentRun;
                    }
                }

                previousChar = bit;
            }
            return longestRun;
        }

        private static string GetConditionalProbKey(int i, int bit, int nextBit)
        {
            return $"AtPosition{i}&CurrentBit={bit}&FrequencyNextBit={nextBit}";
        }

        private static int GetHammingDistance(string s, string t)
        {
            if (s.Length != t.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            int distance =
                s.ToCharArray()
                .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);

            return distance;
        }

        private static void StandardAnalysis(Dictionary<string, double> bitStringLookup, string savePath)
        {
            var maxRecord = bitStringLookup.Aggregate((l, r) => l.Value > r.Value ? l : r);
            var maxCount = maxRecord.Value;
            var maxBitString = maxRecord.Key;
            var minRecord = bitStringLookup.Aggregate((l, r) => l.Value < r.Value ? l : r);
            var minCount = minRecord.Value;
            var minBitString = minRecord.Key;
            var total = bitStringLookup.Values.Sum();
            var bitStringKeys = new List<string>(bitStringLookup.Keys);
            foreach (var key in bitStringKeys)
            {
                bitStringLookup[key] = bitStringLookup[key] / total;
            }
            var notFoundList = bitStringLookup.Where(x => x.Value == 0).Select(x => x.Key).ToList();
            var stats = new FileStatSummary
            {
                BitStringFrequency = bitStringLookup,
                MaxBitString = maxBitString,
                MinBitString = minBitString,
                MaxValue = maxCount,
                MinValue = minCount,
                NotFoundBitStrings = notFoundList,
                NotFoundCount = notFoundList.Count
            };
            var jsonString = JsonSerializer.Serialize(stats);
            File.WriteAllText(Path.Join(savePath, FREQUENCY_FILE_PATH), jsonString);
        }
    }
}

using HonoursGeneratingFunction.GeneratingFunction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HonoursGeneratingFunction.GeneratingFunction.Analysis
{
    public static class ConditionalProbabilityAnalyzer
    {
        public const string CONDITION_FILE_PATH = "conditional.json";

        public static void PerformConditionalProbabilityAnalysis(Dictionary<string, double> bitStringLookup, string savePath)
        {
            var conditionalProbs = new List<ConditionalProbabilityStat>();
            string jsonString;
            var start = 1;
            var end = bitStringLookup.Keys.First().Length;
            var myRange = Enumerable.Range(start, end - start);
            Parallel.ForEach(myRange, bitStringLength =>
            {
                var positionProbs = CalculateProbabilityStatistic(bitStringLookup, bitStringLength, conditionalProbs);
                jsonString = JsonSerializer.Serialize(positionProbs);
                File.WriteAllText(Path.Join(savePath, $"conditional-{bitStringLength}.json"), jsonString);
            });

            //jsonString = JsonSerializer.Serialize(conditionalProbs);
            //File.WriteAllText(Path.Join(savePath, CONDITION_FILE_PATH), jsonString);
        }

        public static ConditionalProbabilityStat CalculateProbabilityStatistic(Dictionary<string, double> bitStringLookup, int bitStringLength, List<ConditionalProbabilityStat> conditionalProbs)
        {
            var subBitStrings = BitStringDictionaryProvider.GeneratePermuations("01", bitStringLength);
            var specificConditionalProbabilities = new List<ConditionalProbabilityStat>((int)Math.Pow(2, bitStringLength));
            foreach (var subBitString in subBitStrings)
            {
                specificConditionalProbabilities.Add(CalculateConditionalProbabilityForString(subBitString, bitStringLookup));
                Console.WriteLine(subBitString);
            }

            var positionProbs = CalculateBitPositionConditionalProbability(specificConditionalProbabilities);
            conditionalProbs.AddRange(specificConditionalProbabilities);
            return positionProbs;
        }

        public static ConditionalProbabilityStat CalculateConditionalProbabilityForString(string substring, Dictionary<string, double> bitStringLookup)
        {
            var totalNextZero = 0d;
            var totalNextOne = 0d;
            var nextPosition = substring.Length;
            var totalMatches = 0d;
            var subBitStrings = bitStringLookup.Where(x => IsSubstring(substring, x.Key));
            foreach (var keyValue in subBitStrings)
            {
                var bitString = keyValue.Key;
                var bitStringCount = keyValue.Value;
                if (bitString.ElementAt(nextPosition) == '0')
                {
                    totalNextZero += bitStringCount;
                }
                if (bitString.ElementAt(nextPosition) == '1')
                {
                    totalNextOne += bitStringCount;
                }
                totalMatches += bitStringCount;
            }

            if (totalMatches == 0)
            {
                return new ConditionalProbabilityStat
                {
                    BitStringSoFar = substring,
                    Position = substring.Length,
                    ConditionalProbDictionary = new Dictionary<string, double>
                {
                    { "NextBit=0", -1 },
                    { "NextBit=1", -1 }
                }
                };
            }

            return new ConditionalProbabilityStat
            {
                BitStringSoFar = substring,
                Position = substring.Length,
                ConditionalProbDictionary = new Dictionary<string, double>
                {
                    { "NextBit=0", totalNextZero / totalMatches },
                    { "NextBit=1", totalNextOne / totalMatches }
                }
            };
        }

        private static ConditionalProbabilityStat CalculateBitPositionConditionalProbability(List<ConditionalProbabilityStat> specificConditionalProbabilityStats)
        {
            var conditionalProbDictionary = new Dictionary<string, double>()
            {
                { GetConditionalProbKey('0', '0'), 0d },
                { GetConditionalProbKey('0', '1'), 0d },
                { GetConditionalProbKey('1', '0'), 0d },
                { GetConditionalProbKey('1', '1'), 0d },
            };
            int position = -1;
            foreach (var bitStringConditionalProb in specificConditionalProbabilityStats)
            {
                position = bitStringConditionalProb.Position - 1;
                var bitString = bitStringConditionalProb.BitStringSoFar;
                if (bitString.ElementAt(position) == '0')
                {
                    conditionalProbDictionary[GetConditionalProbKey('0', '0')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=0"];
                    conditionalProbDictionary[GetConditionalProbKey('0', '1')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=1"];
                }
                if (bitString.ElementAt(position) == '1')
                {
                    conditionalProbDictionary[GetConditionalProbKey('1', '1')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=1"];
                    conditionalProbDictionary[GetConditionalProbKey('1', '0')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=0"];
                }
            }
            var finalProbs = conditionalProbDictionary.ToDictionary(x => x.Key, x => x.Value / specificConditionalProbabilityStats.Count);
            return new ConditionalProbabilityStat
            {
                BitStringSoFar = $"All Strings Length {position + 1}",
                Position = position,
                ConditionalProbDictionary = finalProbs
            };
        }

        private static bool IsSubstring(string subString, string bitString)
        {
            for (var i = 0; i < subString.Length; i++)
            {
                if (bitString.ElementAt(i) != subString.ElementAt(i))
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetConditionalProbKey(char currentBit, char nextBit)
        {
            return $"CurrentBit={currentBit}&NextBit={nextBit}";
        }
    }
}

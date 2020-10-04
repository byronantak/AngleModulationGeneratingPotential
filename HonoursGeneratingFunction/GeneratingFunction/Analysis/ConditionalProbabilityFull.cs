using HonoursGeneratingFunction.GeneratingFunction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HonoursGeneratingFunction.GeneratingFunction.Analysis
{
    public class ConditionalProbabilityFull
    {
        public const string CONDITION_FILE_PATH = "conditional.json";
        private const string CONDITIONAL_PROB_FILE_NAME = "cond-summary.json";

        public static void PerformConditionalProbabilityAnalysis(Dictionary<string, double> bitStringLookup, string savePath)
        {
            var conditionalProbs = new List<ConditionalProbabilityStat>();
            string jsonString;
            var start = 1;
            var end = bitStringLookup.Keys.First().Length;
            var myRange = Enumerable.Range(start, end - start);
            var summaries = new List<ConditionalProbabilitySummary>();
            Parallel.ForEach(myRange, bitStringLength =>
            {
                var positionProbs = CalculateProbabilityStatistic(bitStringLookup, bitStringLength, conditionalProbs);
                jsonString = JsonSerializer.Serialize(positionProbs);
                var frequencyStaySame = positionProbs.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('0', '0')]
                    + positionProbs.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('1', '1')];
                var frequencyDifferent = positionProbs.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('0', '1')]
                   + positionProbs.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('1', '0')];
                summaries.Add(new ConditionalProbabilitySummary
                {
                    Position = positionProbs.Position,
                    ProbabilityBitIsFlipped = frequencyDifferent,
                    ProbabilityBitStaysTheSame = frequencyStaySame
                });

                File.WriteAllText(Path.Join(savePath, $"conditional-{bitStringLength}.json"), jsonString);
            });
            var newJsonContent = JsonSerializer.Serialize(summaries);
            File.WriteAllText(Path.Join(savePath, CONDITIONAL_PROB_FILE_NAME), newJsonContent);

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
            var subBitStrings = bitStringLookup.Where(x => IsSubstring(substring, x.Key)).ToList();
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
                    { "NextBit=0", 0 },
                    { "NextBit=1", 0 }
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
            var totalZero = 0;
            var totalOne = 0;
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
                    totalZero++;
                }
                else if (bitString.ElementAt(position) == '1')
                {
                    conditionalProbDictionary[GetConditionalProbKey('1', '1')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=1"];
                    conditionalProbDictionary[GetConditionalProbKey('1', '0')] +=
                        bitStringConditionalProb.ConditionalProbDictionary["NextBit=0"];
                    totalOne++;
                }
                else
                {
                    throw new Exception("Non Bit character found");
                }
            }
            var finalProbs = conditionalProbDictionary.ToDictionary(x => x.Key, x => x.Value);
            finalProbs[GetConditionalProbKey('0', '0')] /= totalZero;
            finalProbs[GetConditionalProbKey('0', '1')] /= totalZero;
            finalProbs[GetConditionalProbKey('1', '0')] /= totalOne;
            finalProbs[GetConditionalProbKey('1', '1')] /= totalOne;
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

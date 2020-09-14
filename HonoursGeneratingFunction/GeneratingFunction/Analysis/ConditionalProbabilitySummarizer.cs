using HonoursGeneratingFunction.GeneratingFunction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HonoursGeneratingFunction.GeneratingFunction.Analysis
{
    public static class ConditionalProbabilitySummarizer
    {
        private const string CONDITIONAL_PROB_FILE_NAME = "cond-summary.json";

        public static void SummarizeProbabilities(Dictionary<string, double> bitStringLookup, string savePath)
        {
            var bitStringLength = bitStringLookup.Keys.First().Length;
            var conditionProbabilityStats = new List<ConditionalProbabilityStat>(bitStringLength);
            for (var i = 1; i < bitStringLength; i++)
            {
                var countsFilePath = Path.Join(savePath, GetConditionalFileName(i));
                var jsonContent = File.ReadAllText(countsFilePath);
                var probStat = JsonSerializer.Deserialize<ConditionalProbabilityStat>(jsonContent);
                conditionProbabilityStats.Add(probStat);
            }

            var summaries = new List<ConditionalProbabilitySummary>();
            foreach (var stat in conditionProbabilityStats)
            {
                var frequencyStaySame = stat.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('0', '0')] 
                    + stat.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('1', '1')];
                var frequencyDifferent = stat.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('0', '1')]
                   + stat.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('1', '0')];
                //var total = frequencyStaySame + frequencyDifferent;
                summaries.Add(new ConditionalProbabilitySummary
                {
                    Position = stat.Position,
                    ProbabilityBitIsFlipped = frequencyDifferent,
                    ProbabilityBitStaysTheSame = frequencyStaySame
                });
            }
            var newJsonContent = JsonSerializer.Serialize(summaries);
            File.WriteAllText(Path.Join(savePath, CONDITIONAL_PROB_FILE_NAME), newJsonContent);
        }

        private static string GetConditionalFileName(int position)
        {
            return $"conditional-{position}.json";
        }
    }
}

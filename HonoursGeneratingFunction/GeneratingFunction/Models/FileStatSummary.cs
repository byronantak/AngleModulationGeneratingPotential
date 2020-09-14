using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class FileStatSummary
    {
        public double MinValue { get; set; }
        public string MinBitString { get; set; }
        public double MaxValue { get; set; }
        public string MaxBitString { get; set; }

        [JsonIgnore]
        public Dictionary<string, double> BitStringFrequency { get; set; }

        public List<BitStringFrequency> BitStringFrequencyRanking
        {
#pragma warning disable S2365 // Properties should not make collection or array copies
            get => BitStringFrequency.Select(x => new BitStringFrequency
            {
                BitString = x.Key,
                Frequency = x.Value.ToString("0." + new string('#', 339))
            }).OrderByDescending(x => x.Frequency).ToList();
#pragma warning restore S2365 // Properties should not make collection or array copies
        }

        public List<string> NotFoundBitStrings { get; set; }

        public int NotFoundCount { get; set; }
    }
}

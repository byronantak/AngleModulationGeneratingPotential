using System.Collections.Generic;

namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class AdHocStats
    {
        public double FrequencyOfOnesProduced { get; set; }

        public Dictionary<string, double> HammingFrequency { get; set; }

        public List<ConditionalProbabilityStat> ConditionalProbability {get;set;}
    }
}

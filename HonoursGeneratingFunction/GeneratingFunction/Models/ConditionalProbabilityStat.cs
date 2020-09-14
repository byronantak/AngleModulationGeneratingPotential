using System.Collections.Generic;

namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class ConditionalProbabilityStat
    {
        public string BitStringSoFar { get; set; }
        public int Position { get; set; }
        public Dictionary<string, double> ConditionalProbDictionary { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class ConditionalProbabilitySummary
    {
        public int Position { get; set; }
        public double ProbabilityBitIsFlipped { get; set; }
        public double ProbabilityBitStaysTheSame { get; set; }
    }
}

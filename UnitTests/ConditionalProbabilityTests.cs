using HonoursGeneratingFunction.GeneratingFunction.Analysis;
using HonoursGeneratingFunction.GeneratingFunction.Models;
using System.Collections.Generic;
using Xunit;

namespace TestProject
{
    public class ConditionalProbabilityTests
    {
        [Fact]
        public void When_CalculateConditionalProbs_WithEqualCounts_ExpectEqualProbabilities()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", 5 },
                { "01", 5 },
                { "10", 5 },
                { "11", 5 },
            };
            var bitStringLength = 1;
            var myConditionalProbs = new List<ConditionalProbabilityStat>();
            var stat = ConditionalProbabilityAnalyzer.CalculateProbabilityStatistic(myBitStringDictionary, bitStringLength, myConditionalProbs);
            Assert.NotNull(stat);
            Assert.Equal(4, stat.ConditionalProbDictionary.Values.Count);
            foreach (var prob in stat.ConditionalProbDictionary.Values)
            {
                Assert.Equal(0.25, prob);
            }
        }

        [Theory]
        [InlineData(5, 0.25)]
        public void When_CalculateConditionalProbs_WithSkewedCounts_ExpectCorrectProbabilities(double count, double expectedProb)
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", count },
                { "01", 5 },
                { "10", 5 },
                { "11", 5 },
            };
            var bitStringLength = 1;
            var myConditionalProbs = new List<ConditionalProbabilityStat>();
            var stat = ConditionalProbabilityAnalyzer.CalculateProbabilityStatistic(myBitStringDictionary, bitStringLength, myConditionalProbs);
            Assert.NotNull(stat);
            Assert.Equal(4, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal(expectedProb, stat.ConditionalProbDictionary[ConditionalProbabilityAnalyzer.GetConditionalProbKey('0', '0')]);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0.5)]
        [InlineData(15, 0.75)]
        public void When_CalculateConditionalProbsForSpecificString_WithSkewedCounts_ExpectCorrectProbabilities(double count, double expectedProb)
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", count },
                { "01", 5 },
                { "10", 5 },
                { "11", 5 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("0", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal(expectedProb, stat.ConditionalProbDictionary["NextBit=0"]);
        }

        [Fact]
        public void When_CalculateConditionalProbsWithShortString_WithPatternInCountsAndSomeZeroes_ExpectCorrectProbabilities()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "000", 0 },
                { "001", 7 },
                { "010", 0 },
                { "011", 5 },
                { "100", 0 },
                { "101", 7 },
                { "110", 0 },
                { "111", 5 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("0", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal((7d) / (7d + 5) , stat.ConditionalProbDictionary["NextBit=0"]);
            Assert.Equal((5d) / (7d + 5) , stat.ConditionalProbDictionary["NextBit=1"]);
            Assert.Equal(1, stat.ConditionalProbDictionary["NextBit=0"] + stat.ConditionalProbDictionary["NextBit=1"], 2);
        }

        [Fact]
        public void When_CalculateConditionalProbsWithShortString_WithSkewedCountAndSomeZeroes_ExpectCorrectProbabilities()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "000", 0 },
                { "001", 1 },
                { "010", 3 },
                { "011", 0 },
                { "100", 4 },
                { "101", 9 },
                { "110", 0 },
                { "111", 32 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("1", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal((4d + 9) / (4d + 9 + 32), stat.ConditionalProbDictionary["NextBit=0"]);
            Assert.Equal((32) / (4d + 9 + 32), stat.ConditionalProbDictionary["NextBit=1"]);
            Assert.Equal(1, stat.ConditionalProbDictionary["NextBit=0"] + stat.ConditionalProbDictionary["NextBit=1"], 2);
        }

        [Fact]
        public void When_CalculateConditionalProbsWithShortString_WithSkewedCountAndSomeZeroes_LongerBitString_ExpectCorrectProbabilities()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "000", 0 },
                { "001", 7 },
                { "010", 0 },
                { "011", 5 },
                { "100", 0 },
                { "101", 7 },
                { "110", 0 },
                { "111", 5 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("10", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal(0 / 7d, stat.ConditionalProbDictionary["NextBit=0"]);
            Assert.Equal(7 / 7d, stat.ConditionalProbDictionary["NextBit=1"]);
            Assert.Equal(1, stat.ConditionalProbDictionary["NextBit=0"] + stat.ConditionalProbDictionary["NextBit=1"], 2);
        }

        [Theory]
        [InlineData(0, (0 + 5d) / 15)]
        [InlineData(5, (5 + 5d) / 20)]
        [InlineData(15, (5 + 15d) / 30)]
        [InlineData(100, (5 + 100d) / 115)]
        public void When_CalculateConditionalProbsWithShortString_WithSkewedCounts_ExpectCorrectProbabilities(double count, double expectedProb)
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "000", count },
                { "001", 5 },
                { "010", 5 },
                { "011", 5 },
                { "100", 5 },
                { "101", 5 },
                { "110", 5 },
                { "111", 5 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("0", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal(expectedProb, stat.ConditionalProbDictionary["NextBit=0"]);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0.5)]
        [InlineData(15, 0.75)]
        [InlineData(100, 100d / 105)]
        public void When_CalculateConditionalProbsWithLongString_WithSkewedCounts_ExpectCorrectProbabilitiesTe(double count, double expectedProb)
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "000", count },
                { "001", 5 },
                { "010", 5 },
                { "011", 5 },
                { "100", 5 },
                { "101", 5 },
                { "110", 5 },
                { "111", 5 },
            };
            var stat = ConditionalProbabilityAnalyzer.CalculateConditionalProbabilityForString("00", myBitStringDictionary);
            Assert.NotNull(stat);
            Assert.Equal(2, stat.ConditionalProbDictionary.Values.Count);
            Assert.Equal(expectedProb, stat.ConditionalProbDictionary["NextBit=0"]);
        }
    }
}

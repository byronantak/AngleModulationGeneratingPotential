using HonoursGeneratingFunction.GeneratingFunction.Analysis;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class PositionalFrequencyTests
    {
        [Fact]
        public void When_CalculatePositionalFrequenciesProbs_WithAllEqualCounts_ExpectEqualFrequencies()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", 5 },
                { "01", 5 },
                { "10", 5 },
                { "11", 5 },
            };
            var positionsFrequencies = PositionalFrequencyAnalyzer.CalculateFrequencyOfOneForEachPosition(myBitStringDictionary);
            Assert.NotNull(positionsFrequencies);
            Assert.NotEmpty(positionsFrequencies);
            Assert.Equal(0.5, positionsFrequencies.ElementAt(0));
            Assert.Equal(0.5, positionsFrequencies.ElementAt(1));
        }

        [Fact]
        public void When_CalculatePositionalFrequenciesProbs_WithNoCounts_ExpectNaNsForAllFrequencies()
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", 0 },
                { "01", 0 },
                { "10", 0 },
                { "11", 0 },
            };
            var positionsFrequencies = PositionalFrequencyAnalyzer.CalculateFrequencyOfOneForEachPosition(myBitStringDictionary);
            Assert.NotNull(positionsFrequencies);
            Assert.NotEmpty(positionsFrequencies);
            Assert.Equal(double.NaN, positionsFrequencies.ElementAt(0));
            Assert.Equal(double.NaN, positionsFrequencies.ElementAt(1));
        }

        [Theory]
        [InlineData(7, 0, 3, 0, 3d/10, 0)]
        [InlineData(7, 5, 3, 4, (3d + 4)/(7+5+3+4), (5d + 4)/(7+5+3+4))]
        [InlineData(150, 73, 21, 4, (21d + 4)/(150+73+21+4), (73d + 4)/(150+73+21+4))]
        public void When_CalculatePositionalFrequenciesProbs_WithCounts_ExpectCorrectAllFrequencies(double count1, double count2, double count3, double count4, double expectedFreq1, double expectedFreq2)
        {
            var myBitStringDictionary = new Dictionary<string, double>
            {
                { "00", count1 },
                { "01", count2 },
                { "10", count3 },
                { "11", count4 },
            };
            var positionsFrequencies = PositionalFrequencyAnalyzer.CalculateFrequencyOfOneForEachPosition(myBitStringDictionary);
            Assert.NotNull(positionsFrequencies);
            Assert.NotEmpty(positionsFrequencies);
            Assert.Equal(expectedFreq1, positionsFrequencies.ElementAt(0));
            Assert.Equal(expectedFreq2, positionsFrequencies.ElementAt(1));
        }


    }
}

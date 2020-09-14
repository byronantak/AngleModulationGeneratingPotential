using HonoursGeneratingFunction.GeneratingFunction;
using HonoursGeneratingFunction.GeneratingFunction.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HonoursGeneratingFunction
{
    public static class ParameterVisualizer
    {
        public static void Go()
        {
            var parameterResults = new List<ParameterResult>()
            {
                new ParameterResult
                {
                    A = 0,
                    B = 7,
                    BitString = "0000"
                },
                new ParameterResult
                {
                    A = 1,
                    B = 2,
                    BitString = "0110"
                },
                new ParameterResult
                {
                    A = 1,
                    B = 1,
                    BitString = "1111"
                },
            };
            var colors = Visualize(parameterResults, 4);
            var image = BitMapHelper.ToBitmap(colors);
            image.Save(@"C:\Users\byron.antak\Desktop\test3.png");
        }

        public static Color[,] Visualize(List<ParameterResult> parameterResults, int bitStringLength)
        {
            var results = parameterResults.OrderBy(x => x.A).ThenBy(x => x.B);
            var numberOfAs = results.GroupBy(x => x.A).Select(x => x.Count()).Count();
            var numberOfBs = results.GroupBy(x => x.A).Select(x => x.Count()).Max();
            var colors = new Color[numberOfAs, numberOfBs];
            var count = 0;
            for (var i = 0; i < numberOfAs; i++)
            {
                for (var j = 0; j < numberOfBs; j++)
                {
                    if (count < parameterResults.Count)
                    {
                        var oneCount = results.ElementAt(count).OneCount;
                        var color = BitMapHelper.GetColourForNumber(oneCount, bitStringLength);
                        colors[i, j] = color;
                        count++;
                    }
                }
            }
            return colors;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HonoursGeneratingFunction.GeneratingFunction
{
    public static class AngleModulation
    {
        public static string GenerateBitString(IEnumerable<double> samples, double a, double b, double c, double d)
        {
            var currentBitString = new StringBuilder();
            foreach(var sample in samples)
            {
                var bit = GenerateSingleBit(sample, a, b, c, d);
                currentBitString.Append(bit);
            }
            return currentBitString.ToString();
        }

        public static char GenerateSingleBit(double value, double a, double b, double c, double d)
        {
            var angleModulatedValue = G(value, a, b, c, d);
            if (angleModulatedValue > 0)
            {
                return '1';
            }

            return '0';
        }

        public static double G(double x, double a, double b, double c, double d)
        {
            var sharedTerm = 2 * Math.PI * (x - a);
            var cosTerm = Math.Cos(sharedTerm * c);
            var innerTerm = sharedTerm * b * cosTerm;
            return Math.Sin(innerTerm) + d;
        }

    }
}

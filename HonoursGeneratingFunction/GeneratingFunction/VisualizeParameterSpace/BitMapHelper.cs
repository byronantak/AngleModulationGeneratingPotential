using System;
using System.Drawing;

namespace HonoursGeneratingFunction.GeneratingFunction
{
    public static class BitMapHelper
    {
        public static Bitmap ToBitmap(Color[,] rawImage)
        {
            var width = rawImage.GetLength(1);
            var height = rawImage.GetLength(0);
            var image = new Bitmap(width, height);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var color = rawImage[i, j];
                    image.SetPixel(i, j, color);
                }
            }
                
            return image;
        }

        public static Color GetColourForNumber(int value, int maxNumber)
        {
            return GetColourForNumber(Color.Red, Color.Green, value, maxNumber);
        }

        public static Color GetColourForNumber(Color minColor, Color maxColor, int value, int maxNumber)
        {
            var newHue = (maxColor.GetHue() - minColor.GetHue()) * (value - 0) / (maxNumber - 0) + minColor.GetHue();
            return ColorFromHSV(newHue, 1, 1);
        }

        public static void Go(int width, int height)
        {
            var random = new Random(42);

            var colors = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte val = (byte)(random.Next() % 256);
                    var scaledColor = GetColourForNumber(Color.Red, Color.Green, val % 18, 18);
                    colors[x, y] = scaledColor;
                }
            }

            var image = ToBitmap(colors);
            image.Save(@"C:\Users\byron.antak\Desktop\test2.png");
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}

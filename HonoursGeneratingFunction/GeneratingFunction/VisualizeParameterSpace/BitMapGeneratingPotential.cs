using HonoursGeneratingFunction.GeneratingFunction.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HonoursGeneratingFunction.GeneratingFunction
{
    public static class BitMapGeneratingPotential
    {
        public const string RESULTS_FOLDER_NAME = "Results";
        public const string COUNTS_FILE_NAME = "counts.json";
        private const double PARAMETER_VALUE_START = -1;
        private const double PARAMETER_VALUE_END = 1;
        private const double PARAMETER_VALUE_STEP_SIZE = 0.025;
        private const double SAMPLE_STEP_SIZE = 1;
        private const double SAMPLE_START = 1;
        private const bool INCLUDE_D = true;

        // assumes constant arbitrary samples
        public static void ReplicateExperiment(int bitStringSize, string pathToCreateResults)
        {

            var savePath = CreateResultsFolder(pathToCreateResults);
            var resultingFilePath = Path.Join(savePath, COUNTS_FILE_NAME);
            var counter = 0;
            var parameterValues = GenerateValuesInRange(PARAMETER_VALUE_START, PARAMETER_VALUE_END + PARAMETER_VALUE_STEP_SIZE, PARAMETER_VALUE_STEP_SIZE).ToList();
            var samples = CreateSamples(SAMPLE_START, SAMPLE_STEP_SIZE, bitStringSize);
            var bitStringCounter = BitStringDictionaryProvider.CreateBitStringDictionary(bitStringSize, 0);

            var totalIterations = Math.Pow(parameterValues.Count, 3);
            if (INCLUDE_D)
            {
                totalIterations = Math.Pow(parameterValues.Count, 4);
            }

            var parameterResults = new List<ParameterResult>();
            foreach (var a in parameterValues)
            {
                foreach (var b in parameterValues)
                {

                    var bitString = AngleModulation.GenerateBitString(samples, a, b, 0, 0);
                    parameterResults.Add(new ParameterResult
                    {
                        A = a,
                        B = b,
                        C = 0,
                        D = 0,
                        BitString = bitString
                    });
                    bitStringCounter[bitString] += 1;
                    counter += 1;
                    if (counter % 10000 == 0)
                    {
                        Console.WriteLine($"Iteration {counter}/{totalIterations} {counter / totalIterations * 100}%");
                    }

                    //foreach (var c in parameterValues)
                    //{
                    //    if (INCLUDE_D)
                    //    {
                    //        foreach (var d in parameterValues)
                    //        {
                    //            counter = GenerateBitStringIter(counter, samples, bitStringCounter, a, b, c, d, parameterResults);
                    //            if (counter % 10000 == 0)
                    //            {
                    //                Console.WriteLine($"Iteration {counter}/{totalIterations} {counter / totalIterations * 100}%");
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        counter = GenerateBitStringIter(counter, samples, bitStringCounter, a, b, c, 0, parameterResults);
                    //        if (counter % 10000 == 0)
                    //        {
                    //            Console.WriteLine($"Iteration {counter}/{totalIterations} {counter / totalIterations * 100}%");
                    //        }
                    //    }
                    //}
                }
            }

            var colors = ParameterVisualizer.Visualize(parameterResults, bitStringSize);
            var image = BitMapHelper.ToBitmap(colors);
            image.Save(@"C:\Users\byron.antak\Desktop\test4.png");
            Console.WriteLine($"Iteration {counter}/{totalIterations} {counter / totalIterations * 100}%");
            var jsonString = JsonSerializer.Serialize(bitStringCounter);
            File.WriteAllText(resultingFilePath, jsonString);
        }

        private static int GenerateBitStringIter(int counter, List<double> samples, Dictionary<string, int> bitStringCounter, double a, double b, double c, double d, List<ParameterResult> parameterResults)
        {
            return 0;
        }

        private static string CreateResultsFolder(string resultFilePath)
        {
            var fullPath = Path.Join(resultFilePath, RESULTS_FOLDER_NAME);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            return fullPath;
        }

        public static List<double> CreateSamples(double start, double stepSize, int size)
        {
            var list = new List<double>(size);
            for (var i = 0; i < size; i++)
            {
                list.Add(start + (i * stepSize));
            }
            return list;
        }

        public static IEnumerable<double> GenerateValuesInRange(double start, double stop, double step)
        {
            var x = start;
            do
            {
                yield return x;
                x += step;
                if (step < 0 && x <= stop || 0 < step && stop <= x)
                    break;
            }
            while (true);
        }
    }
}

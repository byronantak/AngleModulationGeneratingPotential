using HonoursGeneratingFunction.GeneratingFunction;
using HonoursGeneratingFunction.GeneratingFunction.Configuration;
using Microsoft.Extensions.Configuration;

namespace HonoursGeneratingFunction
{
    public static class Program
    {
        private static void Main()
        {
            var configBuilder = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .Build();

            var config = new AppConfiguration
            {
                BitStringLength = int.Parse(configBuilder.GetSection("BitStringLength").Value),
                ResultFilePath = configBuilder.GetSection("ResultFilePath").Value
            };

            GeneratingPotential.PerformExperiment(config.BitStringLength, config.ResultFilePath, null, null, null, null);
            ResultInterpreter.Interpret(config.BitStringLength, config.ResultFilePath);
        }
    }
}

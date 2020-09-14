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

            GeneratingPotential.ReplicateExperiment(config.BitStringLength, config.ResultFilePath);
            ResultInterpreter.Interpret(config.BitStringLength, config.ResultFilePath);
        }
    }
}

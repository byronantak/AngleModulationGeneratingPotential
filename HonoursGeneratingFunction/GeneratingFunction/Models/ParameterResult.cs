using System.Linq;

namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class ParameterResult
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public string BitString { get; set; }
        public int OneCount
        {
            get
            {
                return BitString.Count(x => x == '1');
            }
        }
    }
}

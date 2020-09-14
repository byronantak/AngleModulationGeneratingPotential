namespace HonoursGeneratingFunction.GeneratingFunction.Models
{
    public class BitStringFrequency
    {
        public string BitString { get; set; }
        public string Frequency { get; set; }

        public override string ToString()  
        {
            return $"{BitString} - {Frequency}";
        }
    }
}

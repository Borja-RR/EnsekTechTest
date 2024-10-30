namespace EnsekTechTest.Repository.Sources
{
    public class MeterReading
    {
        // all strings as this comes from a CSV file
        public string? AccountId { get; set; }
        public string? MeterReadingDateTime { get; set; }
        public string? MeterReadValue { get; set; }
    }
}

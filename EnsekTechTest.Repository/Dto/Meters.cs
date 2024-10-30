namespace EnsekTechTest.Repository.Dto
{
    public class Meters
    {
        public long AccountId { get; set; }
        public DateTime LastMeterReadingTime { get; set; }
        public long MeterReadValue { get; set; }
        
        // Navigation property
        public Accounts Account { get; set; }

    }
}

using EnsekTechTest.Repository.Dto;

namespace EnsekTechTest.Services.Models
{
    public class ParsedMeterReadings
    {
        public List<Meters> Meters { get; set; } = new List<Meters>();
        public int FailedRecords { get; set; }
    }
}
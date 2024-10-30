using EnsekTechTest.Repository.Sources;
using EnsekTechTest.Services.Models;

namespace EnsekTechTest.Services.Meter.Interfaces
{
    public interface IMeterReadingValidationService
    {
        ParsedMeterReadings ValidateMeterReadings(List<MeterReading> readings);
    }
}

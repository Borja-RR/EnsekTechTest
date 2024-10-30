using EnsekTechTest.Repository.Sources;
using EnsekTechTest.Services.CsvHelper.Interfaces;
using EnsekTechTest.Services.Meter.Interfaces;
using EnsekTechTest.Services.Models;
using Microsoft.AspNetCore.Http;

namespace EnsekTechTest.Services.Meter
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly ICsvService _csvService;
        private readonly IMeterReadingValidationService _meterReadingValidationService;

        public MeterReadingService(ICsvService csvService,
            IMeterReadingValidationService meterReadingValidationService)
        {
            _csvService = csvService;
            _meterReadingValidationService = meterReadingValidationService;
        }

        public async Task<ParsedMeterReadings> GetParsedMeterReadingsAsync(IFormFile file)
        {
            var readings = await Task.Run(() => _csvService.ReadCsv<MeterReading>(file.OpenReadStream()));

            var parsedMeterReadings = _meterReadingValidationService.ValidateMeterReadings(readings);

            return parsedMeterReadings;
        }
    }
}

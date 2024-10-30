using EnsekTechTest.Services.Models;
using Microsoft.AspNetCore.Http;

namespace EnsekTechTest.Services.Meter.Interfaces
{
    public interface IMeterReadingService
    {
        Task<ParsedMeterReadings> GetParsedMeterReadingsAsync(IFormFile file);
    }
}

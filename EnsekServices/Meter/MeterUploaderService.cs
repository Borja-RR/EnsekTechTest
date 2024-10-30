using EnsekTechTest.Repository.Repository.Interfaces;
using EnsekTechTest.Services.Meter.Interfaces;
using EnsekTechTest.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EnsekTechTest.Services.Meter
{
    public class MeterUploaderService : IMeterUploaderService
    {
        private readonly ILogger<MeterUploaderService> _logger;
        private readonly IMeterReadingService _meterReadingService;
        private readonly IMetersRepository _metersRepository;

        public MeterUploaderService(
            ILogger<MeterUploaderService> logger,
            IMeterReadingService meterReadingService,
            IMetersRepository metersRepository)
        {
            _logger = logger;
            _meterReadingService = meterReadingService;
            _metersRepository = metersRepository;
        }

        public async Task<UploaderResult> MetersUploadAsync(IFormFile file)
        {
            if (file == null)
            {
                _logger.LogWarning("No files were provided in the upload request.");
                return new UploaderResult { SuccessfullRecords = 0, FailedRecords = 0 };
            }

            var parsedResult = await _meterReadingService.GetParsedMeterReadingsAsync(file!);
            var meterUploadResult = await _metersRepository.UploadMetersAsync(parsedResult.Meters, parsedResult.FailedRecords);

            _logger.LogInformation("Meter readings upload completed. Successful records: {SuccessRecords}, Failed records: {FailedRecords}.",
                    meterUploadResult.SuccessRecords, parsedResult.FailedRecords);

            return new UploaderResult()
            {
                SuccessfullRecords = meterUploadResult.SuccessRecords,
                FailedRecords = meterUploadResult.FailedRecords
            };
        }
    }
}

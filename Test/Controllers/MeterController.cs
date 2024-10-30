using EnsekTechTest.Services.Meter.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeterController : ControllerBase
    {
        private readonly ILogger<MeterController> _logger;
        private readonly IMeterUploaderService _meterUploaderService;
        
        public MeterController(ILogger<MeterController> logger, IMeterUploaderService meterUploaderService)
        {
            _logger = logger;
            _meterUploaderService = meterUploaderService;
        }

        [HttpPost]
        [Route("meter-reading-uploads")]
        public async Task<IActionResult> MeterReadingUploads([FromForm] IFormFileCollection file)
        {
            if (file == null || file.Count() == 0 || file.Count() > 1)
            {
                _logger.LogWarning("No file uploaded in the request");
                return BadRequest("No file uploaded.");
            }

            try
            {
                var result = await _meterUploaderService.MetersUploadAsync(file[0]);

                if (result == null)
                {
                    _logger.LogError("File processing returned null");
                    return StatusCode(500, "An error ocurred during file processing.");
                }

                return Ok(result);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Error during file upload.");
                return StatusCode(500, "An error ocurred during file processing");
            }
            
        }
    }
}

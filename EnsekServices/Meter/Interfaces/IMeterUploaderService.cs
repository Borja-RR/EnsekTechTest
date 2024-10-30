using EnsekTechTest.Services.Models;
using Microsoft.AspNetCore.Http;

namespace EnsekTechTest.Services.Meter.Interfaces
{
    public interface IMeterUploaderService
    {
        Task<UploaderResult> MetersUploadAsync(IFormFile files);
    }
}

using EnsekTechTest.Repository.Dto;
using EnsekTechTest.Repository.Models;

namespace EnsekTechTest.Repository.Repository.Interfaces
{
    public interface IMetersRepository
    {
        Task<Accounts> GetAccountAsync(int accountId);
        Task<MeterUploadResult> UploadMetersAsync(List<Meters> meters, int failedRecords);
    }
}

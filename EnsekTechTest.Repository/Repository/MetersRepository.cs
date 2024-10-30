using EnsekTechTest.Repository.Dto;
using EnsekTechTest.Repository.Models;
using EnsekTechTest.Repository.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EnsekTechTest.Repository.Repository
{
    public class MetersRepository : IMetersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public MetersRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Accounts> GetAccountAsync(int accountId)
        {
            return await _context.Accounts.FindAsync(accountId);
        }

        public async Task<MeterUploadResult> UploadMetersAsync(List<Meters> meters, int failedRecords)
        {
            var successRecords = 0;

            try
            {
                foreach (var meter in meters)
                {
                    // Check if the associated Account exists
                    var account = _context.Accounts.FirstOrDefault(a => a.AccountId == meter.AccountId);
                    if (account == null)
                    {
                        // Skip or handle the case where the account does not exist
                        Console.WriteLine($"Account with ID {meter.AccountId} does not exist.");
                        failedRecords++;
                        continue;
                    }

                    // Find existing meter with matching ID (or use other unique identifier)
                    var existingMeter = _context.Meters.FirstOrDefault(m => m.AccountId == meter.AccountId);

                    if (existingMeter == null)
                    {
                        await _context.Meters.AddAsync(meter);
                        successRecords++;
                    }
                    else if (existingMeter.LastMeterReadingTime < meter.LastMeterReadingTime)
                    {
                        // Update properties selectively
                        existingMeter.MeterReadValue = meter.MeterReadValue;
                        existingMeter.LastMeterReadingTime = meter.LastMeterReadingTime;

                        successRecords++;
                    }
                    else
                        failedRecords++;
                }

                // Save all changes to the database at once
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading meterReadings: {ex.Message}");
                throw;
            }

            return new MeterUploadResult()
            {
                FailedRecords = failedRecords,
                SuccessRecords = successRecords
            };
        }
    }
}

using EnsekTechTest.Repository.Dto;
using EnsekTechTest.Repository.Sources;
using EnsekTechTest.Services.Meter.Interfaces;
using EnsekTechTest.Services.Models;
using System.Text.RegularExpressions;

namespace EnsekTechTest.Services.Meter
{
    public class MeterReadingValidationService : IMeterReadingValidationService
    {
        public ParsedMeterReadings ValidateMeterReadings(List<MeterReading> readings)
        {
            var meters = new List<Meters>();
            int failedRecords = 0;
            var processedAccountIds = new HashSet<long>(); // Use HashSet for faster duplicate checks

            foreach (var reading in readings)
            {
                try
                {
                    // validate 'Account' format
                    if (!long.TryParse(reading.AccountId, out long accountId))
                    {
                        failedRecords++;
                        continue;
                    }

                    // Check for duplicate AccountId within the file
                    if (processedAccountIds.Contains(accountId))
                    {
                        failedRecords++;
                        continue;
                    }

                    // Validate 'value' is of NNNNN format (not sure if negatives are valid)
                    if (reading.MeterReadValue == null || !Regex.IsMatch(reading.MeterReadValue.ToString(), @"^-?[0-9]{1,5}$"))
                    {
                        failedRecords++;
                        continue;
                    }

                    // checks dateTime format
                    var dateValid = DateTime.TryParse(reading.MeterReadingDateTime, out var meterReadingDateTime);

                    if (dateValid && !meters.Exists(a => a.AccountId == accountId))    //extra validation in case duplication of AccountId within the file
                    {
                        var meter = new Meters()
                        {
                            AccountId = accountId,
                            LastMeterReadingTime = meterReadingDateTime,
                            MeterReadValue = int.Parse(reading.MeterReadValue)    //we can safely parse after previous validation
                        };

                        meters.Add(meter);
                        processedAccountIds.Add(accountId); // Track AccountId to avoid duplicates
                    }
                    else
                    {
                        failedRecords++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during validation: {ex.ToString()");
                    throw;
                }
            }

            return new ParsedMeterReadings()
            {
                FailedRecords = failedRecords,
                Meters = meters
            };
        }
    }
}

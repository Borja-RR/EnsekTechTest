using EnsekTechTest.Repository;
using EnsekTechTest.Repository.Dto;
using EnsekTechTest.Repository.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace EnsekTechTest.UnitTests
{
    public class MetersRepositoryTests
    {
        private DataContext CreateSQLiteInMemoryDataContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("DataSource=:memory:") // Use SQLite in-memory database
                .Options;

            var context = new DataContext(options);
            context.Database.OpenConnection(); // Keep the connection open to maintain in-memory database
            context.Database.EnsureCreated();  // Ensure schema is created for in-memory testing
            return context;
        }

        [Fact]
        public async Task UploadMetersAsync_ShouldAddNewMeterRecord_WhenAccountExistsAndNoPreviousReading()
        {
            // Arrange
            using var context = CreateSQLiteInMemoryDataContext();
            var repository = new MetersRepository(context);

            // Seed the account in the database
            context.Accounts.Add(new Accounts { AccountId = 1, FirstName = "First Name", Lastname = "Last Name" });
            await context.SaveChangesAsync();

            var meters = new List<Meters>
        {
            new Meters { AccountId = 1, LastMeterReadingTime = DateTime.UtcNow, MeterReadValue = 12345 }
        };
            int failedRecords = 0;

            // Act
            var meterUploadResult = await repository.UploadMetersAsync(meters, failedRecords);

            // Assert
            Assert.Equal(1, meterUploadResult.SuccessRecords);
            Assert.Equal(0, meterUploadResult.FailedRecords);
            Assert.Single(context.Meters); // Verify meter was added
        }

        [Fact]
        public async Task UploadMetersAsync_ShouldIncrementFailedRecords_WhenAccountDoesNotExist()
        {
            // Arrange
            using var context = CreateSQLiteInMemoryDataContext();
            var repository = new MetersRepository(context);

            var meters = new List<Meters>
            {
                new Meters { AccountId = 2, LastMeterReadingTime = DateTime.UtcNow, MeterReadValue = 12345 }
            };
            int failedRecords = 0;

            // Act
            var meterUploadResult = await repository.UploadMetersAsync(meters, failedRecords);

            // Assert
            Assert.Equal(0, meterUploadResult.SuccessRecords);
            Assert.Equal(1, meterUploadResult.FailedRecords);
        }

        [Fact]
        public async Task UploadMetersAsync_ShouldUpdateMeterRecord_WhenReadingIsNewer()
        {
            // Arrange
            using var context = CreateSQLiteInMemoryDataContext();
            var repository = new MetersRepository(context);

            context.Accounts.Add(new Accounts { AccountId = 1, FirstName = "First Name", Lastname = "Last Name" });
            context.Meters.Add(new Meters
            {
                AccountId = 1,
                LastMeterReadingTime = DateTime.UtcNow.AddDays(-1),
                MeterReadValue = 10000
            });
            await context.SaveChangesAsync();

            var meters = new List<Meters>
        {
            new Meters { AccountId = 1, LastMeterReadingTime = DateTime.UtcNow, MeterReadValue = 20000 }
        };
            int failedRecords = 0;

            // Act
            var meterUploadResult = await repository.UploadMetersAsync(meters, failedRecords);

            // Assert
            Assert.Equal(1, meterUploadResult.SuccessRecords);
            Assert.Equal(0, meterUploadResult.FailedRecords);

            var updatedMeter = await context.Meters.FirstOrDefaultAsync(m => m.AccountId == 1);
            Assert.NotNull(updatedMeter);
            Assert.Equal(20000, updatedMeter.MeterReadValue); // Verify updated reading
        }

        [Fact]
        public async Task UploadMetersAsync_ShouldIncrementFailedRecords_WhenReadingIsOlder()
        {
            // Arrange
            using var context = CreateSQLiteInMemoryDataContext();
            var repository = new MetersRepository(context);

            context.Accounts.Add(new Accounts { AccountId = 1, FirstName = "First Name", Lastname = "Last Name" });
            context.Meters.Add(new Meters
            {
                AccountId = 1,
                LastMeterReadingTime = DateTime.UtcNow,
                MeterReadValue = 20000
            });
            await context.SaveChangesAsync();

            var meters = new List<Meters>
            {
                new Meters { AccountId = 1, LastMeterReadingTime = DateTime.UtcNow.AddDays(-1), MeterReadValue = 10000 }
            };
            int failedRecords = 0;

            // Act
            var meterUploadResult = await repository.UploadMetersAsync(meters, failedRecords);

            // Assert
            Assert.Equal(0, meterUploadResult.SuccessRecords);
            Assert.Equal(1, meterUploadResult.FailedRecords);
        }

        [Fact]
        public async Task UploadMetersAsync_ShouldThrowException_WhenDatabaseErrorOccurs()
        {
            // Arrange
            var contextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            await using var context = new DataContext(contextOptions);
            var repository = new MetersRepository(context);

            // Dispose context to simulate an error on SaveChangesAsync
            await context.Database.CloseConnectionAsync();

            var meters = new List<Meters>
            {
                new Meters { AccountId = 1, LastMeterReadingTime = DateTime.UtcNow, MeterReadValue = 12345 }
            };
            int failedRecords = 0;

            // Act & Assert
            await Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() => repository.UploadMetersAsync(meters, failedRecords));
        }
    }
}
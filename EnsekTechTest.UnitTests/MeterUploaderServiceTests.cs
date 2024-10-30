using EnsekTechTest.Repository.Dto;
using EnsekTechTest.Repository.Models;
using EnsekTechTest.Repository.Repository.Interfaces;
using EnsekTechTest.Services.Meter;
using EnsekTechTest.Services.Meter.Interfaces;
using EnsekTechTest.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace EnsekTechTest.UnitTests
{
    public class MeterUploaderServiceTests
    {
        private readonly Mock<ILogger<MeterUploaderService>> _loggerMock;
        private readonly Mock<IMeterReadingService> _meterReadingServiceMock;
        private readonly Mock<IMetersRepository> _metersRepositoryMock;
        private readonly MeterUploaderService _service;

        public MeterUploaderServiceTests()
        {
            _loggerMock = new Mock<ILogger<MeterUploaderService>>();
            _meterReadingServiceMock = new Mock<IMeterReadingService>();
            _metersRepositoryMock = new Mock<IMetersRepository>();
            _service = new MeterUploaderService(_loggerMock.Object, _meterReadingServiceMock.Object, _metersRepositoryMock.Object);
        }

        [Fact]
        public async Task MetersUploadAsync_ShouldReturnZeroRecords_WhenFileIsNull()
        {
            // Act
            var result = await _service.MetersUploadAsync(null);

            // Assert
            Assert.Equal(0, result.SuccessfullRecords);
            Assert.Equal(0, result.FailedRecords);
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No files were provided")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task MetersUploadAsync_ShouldProcessSuccessfully_WhenAllRecordsAreValid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var parsedResult = new ParsedMeterReadings
            {
                Meters = new List<Meters> { new Meters { AccountId = 1, LastMeterReadingTime = System.DateTime.UtcNow, MeterReadValue = 12345 } },
                FailedRecords = 0
            };

            _meterReadingServiceMock.Setup(service => service.GetParsedMeterReadingsAsync(fileMock.Object))
                .ReturnsAsync(parsedResult);

            var meterUploadResult = new MeterUploadResult { SuccessRecords = 1, FailedRecords = 0 };
            _metersRepositoryMock.Setup(repo => repo.UploadMetersAsync(parsedResult.Meters, It.IsAny<int>()))
                .ReturnsAsync(meterUploadResult);

            // Act
            var result = await _service.MetersUploadAsync(fileMock.Object);

            // Assert
            Assert.Equal(1, result.SuccessfullRecords);
            Assert.Equal(0, result.FailedRecords);
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Meter readings upload completed. Successful records: {meterUploadResult.SuccessRecords}, Failed records: {meterUploadResult.FailedRecords}")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task MetersUploadAsync_ShouldReturnFailedRecords_WhenParsingFails()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var parsedResult = new ParsedMeterReadings
            {
                Meters = new List<Meters>(),
                FailedRecords = 3 // Simulate parsing failure
            };

            _meterReadingServiceMock.Setup(service => service.GetParsedMeterReadingsAsync(fileMock.Object))
                .ReturnsAsync(parsedResult);

            var meterUploadResult = new MeterUploadResult { SuccessRecords = 0, FailedRecords = parsedResult.FailedRecords };
            _metersRepositoryMock.Setup(repo => repo.UploadMetersAsync(parsedResult.Meters, It.IsAny<int>()))
                .ReturnsAsync(meterUploadResult);

            // Act
            var result = await _service.MetersUploadAsync(fileMock.Object);

            // Assert
            Assert.Equal(0, result.SuccessfullRecords);
            Assert.Equal(3, result.FailedRecords);
        }
    }
}
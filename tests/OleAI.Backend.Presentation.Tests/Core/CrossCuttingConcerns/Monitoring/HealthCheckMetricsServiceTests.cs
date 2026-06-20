using System;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OleAI.Backend.Presentation.Core.CrossCuttingConcerns.Monitoring;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Core.CrossCuttingConcerns.Monitoring
{
    public class HealthCheckMetricsServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldCheckHealth_WhenStarted()
        {
            var mockHealthCheckService = new Mock<HealthCheckService>();
            var report = new HealthReport(
                new System.Collections.Generic.Dictionary<string, HealthReportEntry>(),
                TimeSpan.FromSeconds(1));

            mockHealthCheckService.Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(report);

            var mockMeterFactory = new Mock<IMeterFactory>();
            mockMeterFactory.Setup(m => m.Create(It.IsAny<MeterOptions>()))
                            .Returns(new Meter("OleAI.HealthChecks"));

            var mockLogger = new Mock<ILogger<HealthCheckMetricsService>>();

            var service = new HealthCheckMetricsService(mockHealthCheckService.Object, mockMeterFactory.Object, mockLogger.Object);

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            var hostedService = (IHostedService)service;
            await hostedService.StartAsync(cts.Token);

            await Task.Delay(200);

            try { await hostedService.StopAsync(CancellationToken.None); } catch { }

            mockHealthCheckService.Verify(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleException()
        {
            var mockHealthCheckService = new Mock<HealthCheckService>();
            mockHealthCheckService.Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(), It.IsAny<CancellationToken>()))
                                  .ThrowsAsync(new Exception("Test Exception"));

            var mockMeterFactory = new Mock<IMeterFactory>();
            mockMeterFactory.Setup(m => m.Create(It.IsAny<MeterOptions>()))
                            .Returns(new Meter("OleAI.HealthChecks"));

            var mockLogger = new Mock<ILogger<HealthCheckMetricsService>>();

            var service = new HealthCheckMetricsService(mockHealthCheckService.Object, mockMeterFactory.Object, mockLogger.Object);

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            var hostedService = (IHostedService)service;
            await hostedService.StartAsync(cts.Token);

            await Task.Delay(200);

            try { await hostedService.StopAsync(CancellationToken.None); } catch { }

            mockHealthCheckService.Verify(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }
    }
}

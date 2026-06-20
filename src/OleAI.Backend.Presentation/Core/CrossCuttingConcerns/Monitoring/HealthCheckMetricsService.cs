using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OleAI.Backend.Presentation.Core.CrossCuttingConcerns.Monitoring;

public class HealthCheckMetricsService : BackgroundService
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckMetricsService> _logger;
    private int _isHealthy = 1;

    public HealthCheckMetricsService(HealthCheckService healthCheckService, IMeterFactory meterFactory, ILogger<HealthCheckMetricsService> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;

        var meter = meterFactory.Create("OleAI.HealthChecks");
        meter.CreateObservableGauge("healthcheck_status", () => _isHealthy, description: "System Health Status (1=Healthy, 0=Unhealthy)");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(stoppingToken);
                _isHealthy = report.Status == HealthStatus.Healthy ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking health.");
                _isHealthy = 0;
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
